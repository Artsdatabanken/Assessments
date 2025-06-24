using System.Net;
using Assessments.Shared.Constants;
using Assessments.Shared.Interfaces;
using Assessments.Shared.Repositories;
using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Timeout;
using RodlisteNaturtyper.Data;

namespace Assessments.Shared;

public static class SharedModule
{
    public static void AddSharedModule(this IServiceCollection services, IConfigurationRoot configuration)
    {
        services.AddLazyCache(_ =>
        {
            var cache = new CachingService(CachingService.DefaultCacheProvider)
            {
                DefaultCachePolicy =
                {
                    DefaultCacheDurationSeconds = (int) TimeSpan.FromDays(1).TotalSeconds
                }
            };
            return cache;
        });

        services.ConfigureHttpClientDefaults(builder =>
        {
            builder.SetHandlerLifetime(TimeSpan.FromMinutes(60));
        });

        services.AddHttpClient(string.Empty).AddStandardResilienceHandler();
        
        services.AddHttpClient<IDrupalRepository, DrupalRepository>()
            .AddStandardResilienceHandler(options =>
            {
                options.Retry.ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .Handle<TimeoutRejectedException>()
                    .HandleResult(response => response.StatusCode == HttpStatusCode.BadGateway)
                    .HandleResult(response => response.StatusCode == HttpStatusCode.GatewayTimeout);
            });

        services.AddHttpClient<INinKodeRepository, NinKodeRepository>().AddStandardResilienceHandler();
        
        services.AddDbContext<RodlisteNaturtyperDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString(ConnectionStrings.RodlisteForNaturtyper);
            
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException($"ConnectionString '{ConnectionStrings.RodlisteForNaturtyper}' not found");

            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            options.UseSqlServer(connectionString, builder =>
            {
                builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                builder.EnableRetryOnFailure();
            });
        });

        services.AddScoped<INatureTypesRepository, NatureTypesRepository>();
    }
}