using System.Globalization;
using System.Text.Json.Serialization;
using Assessments.Data;
using Assessments.Shared;
using Assessments.Web.Infrastructure;
using Assessments.Web.Infrastructure.AlienSpecies;
using Assessments.Web.Infrastructure.Middleware;
using Assessments.Web.Infrastructure.Services;
using Assessments.Shared.Options;
using Azure.Identity;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using NLog.Web;
using RobotsTxt;
using SendGrid.Extensions.DependencyInjection;
using Microsoft.AspNetCore.HttpOverrides;

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

builder.Services.Configure<RouteOptions>(options => { options.LowercaseUrls = true; });

builder.Services.AddControllersWithViews()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
    .AddViewLocalization(options => options.ResourcesPath = "Resources")
    .AddOData(ODataHelper.Options);

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var cultures = new List<CultureInfo>
    {
        new("no"),
        new("en")
    };

    options.DefaultRequestCulture = new RequestCulture(cultures.First());
    
    options.SupportedCultures = cultures;
    options.SupportedUICultures = cultures;
    
    options.RequestCultureProviders.Remove(typeof(AcceptLanguageHeaderRequestCultureProvider));
});

builder.Services.AddScoped<RequestLocalizationCookiesMiddleware>();

builder.Services.AddDbContext<AssessmentsDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default") ?? throw new InvalidOperationException(), providerOptions => providerOptions.MigrationsAssembly(typeof(AssessmentsDbContext).Assembly.FullName).EnableRetryOnFailure());
});

builder.Services.AddSharedModule();

builder.Services.AddSingleton<DataRepository>();

builder.Services.AddSingleton<AttachmentRepository>();

builder.Services.AddTransient<ExpertCommitteeMemberService>();

builder.Services.AddHttpClient<ArtskartApiService>();

builder.Services.AddAutoMapper(cfg => cfg.AddMaps(Constants.AssessmentsMappingAssembly));

builder.Services.AddResponseCompression(options => options.EnableForHttps = true);

var applicationOptions = builder.Configuration.GetSection(nameof(ApplicationOptions));

builder.Services.AddOptions<ApplicationOptions>().Bind(applicationOptions).ValidateDataAnnotations().ValidateOnStart();

builder.Services.AddSendGrid(options => { options.ApiKey = applicationOptions.Get<ApplicationOptions>().SendGridApiKey; });

if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddDataProtection().SetApplicationName("Assessments").PersistKeysToDbContext<AssessmentsDbContext>();
}
else
{
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();
}

builder.Services.AddStaticRobotsTxt(options =>
{
    options.DenyAll(); // TODO: endre når nye sider er tatt i bruk

    //if (!builder.Environment.IsProduction())
    //    options.DenyAll();

    return options;
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost | ForwardedHeaders.XForwardedHost;

    options.ForwardedHostHeaderName = "X-Original-Host";

    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

app.UseForwardedHeaders();

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

ExportHelper.Setup();

// TODO: legge til en innstilling om man skal jobbe med databasen lokalt?
if (!app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dataContext = scope.ServiceProvider.GetRequiredService<AssessmentsDbContext>();
    dataContext.Database.Migrate();
}

app.Run();