using System.Globalization;
using System.Text.Json.Serialization;
using Assessments.Data;
using Assessments.Shared;
using Assessments.Shared.Constants;
using Assessments.Shared.Extensions;
using Assessments.Shared.Options;
using Assessments.Web.Infrastructure;
using Assessments.Web.Infrastructure.AlienSpecies;
using Assessments.Web.Infrastructure.Middleware;
using Assessments.Web.Infrastructure.Services;
using Azure.Identity;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement;
using NLog.Web;
using RobotsTxt;
using SendGrid.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseNLog();

if (!builder.Environment.IsDevelopment())
{
    builder.Configuration.AddAzureKeyVault(
        new Uri($"https://{builder.Configuration["KeyVault:Name"]}.vault.azure.net/"),
        new DefaultAzureCredential());

    builder.Services.AddApplicationInsightsTelemetry();
    builder.Services.AddSingleton<ITelemetryInitializer, TelemetryInitializer>();
}

builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

builder.Services.AddControllersWithViews()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
    .AddViewLocalization(options => options.ResourcesPath = "Resources")
    .AddOData(options => options.EnableQueryFeatures(maxTopValue: 100).AddRouteComponents("odata/v1", ODataHelper.GetModel()));

builder.Services.AddDbContext<AssessmentsDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString(ConnectionStrings.Default) ?? throw new InvalidOperationException($"ConnectionString '{ConnectionStrings.Default}' not found"), providerOptions => providerOptions.MigrationsAssembly(typeof(AssessmentsDbContext).Assembly.FullName).EnableRetryOnFailure()));

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var cultures = new List<CultureInfo> { new("no"), new("en") };

    options.DefaultRequestCulture = new RequestCulture(cultures.First());
    options.SupportedCultures = cultures;
    options.SupportedUICultures = cultures;
    options.RequestCultureProviders.Remove(typeof(AcceptLanguageHeaderRequestCultureProvider));
    options.RequestCultureProviders.OfType<CookieRequestCultureProvider>().First().CookieName = "adb.req.culture";
});

builder.Services.AddAntiforgery(options => options.Cookie.Name = "adb.req.antiforgery");

builder.Services.AddScoped<RequestLocalizationCookiesMiddleware>();

builder.Services.AddProblemDetails(options =>
    options.CustomizeProblemDetails = ctx =>
        ctx.ProblemDetails.Extensions.Add("environment", builder.Environment.EnvironmentName));

builder.Services.AddSharedModule(builder.Configuration);

builder.Services.AddSingleton<DataRepository>();

builder.Services.AddSingleton<AttachmentRepository>();

builder.Services.AddTransient<ExpertCommitteeMemberService>();

builder.Services.AddHttpClient<ArtskartApiService>();

builder.Services.AddAutoMapper(cfg => cfg.AddMaps(Constants.AssessmentsMappingAssembly));

builder.Services.AddResponseCompression(options => options.EnableForHttps = true);

var applicationOptions = builder.Configuration.GetSection(nameof(ApplicationOptions));

builder.Services.AddOptions<ApplicationOptions>().Bind(applicationOptions).ValidateDataAnnotations().ValidateOnStart();

builder.Services.AddSendGrid(options => options.ApiKey = applicationOptions.Get<ApplicationOptions>().SendGridApiKey);

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();
}

builder.Services.AddStaticRobotsTxt(options =>
{
    if (!builder.Environment.IsProduction())
        options.DenyAll();

    return options;
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost | ForwardedHeaders.XForwardedHost;
    options.ForwardedHostHeaderName = "X-Original-Host";
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddCors(options => { options.AddPolicy(name: CorsConstants.AllowAny, policy =>
{
    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
}); });

builder.Services.AddFeatureManagement();

builder.Services.AddSwagger(builder.Environment);

var app = builder.Build();

app.UseForwardedHeaders();

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseODataRouteDebug();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
    app.UseResponseCompression();
}

app.HandleApiException();

app.UseStatusCodePagesWithReExecute("/Error/{0}");

app.UseRequestLocalization();

app.UseRequestLocalizationCookies();

app.MapStaticAssets();

app.UseCookiePolicy(new CookiePolicyOptions
{
    Secure = CookieSecurePolicy.Always,
    HttpOnly = HttpOnlyPolicy.Always,
    MinimumSameSitePolicy = SameSiteMode.Strict
});

var cachedFilesFolder = Path.Combine(app.Environment.ContentRootPath, Constants.CacheFolder);

if (!Directory.Exists(cachedFilesFolder))
    Directory.CreateDirectory(cachedFilesFolder);

app.MapDefaultControllerRoute().WithStaticAssets();

app.UseRobotsTxt();

app.ConfigureSwagger(builder.Environment);

ExportHelper.Setup();

if (!app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dataContext = scope.ServiceProvider.GetRequiredService<AssessmentsDbContext>();
    dataContext.Database.Migrate();
}

app.Run();