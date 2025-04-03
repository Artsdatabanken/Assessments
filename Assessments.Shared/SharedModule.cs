using Assessments.Shared.Interfaces;
using LazyCache;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Assessments.Shared.Repositories;
using Polly;
using Polly.Timeout;

namespace Assessments.Shared;

public static class SharedModule
{
    public static void AddSharedModule(this IServiceCollection services)
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

        services.AddScoped<INatureTypesRepository, NatureTypesRepository>();

        services.AddHttpClient<IDrupalRepository, DrupalRepository>()
            .AddStandardResilienceHandler(options =>
            {
                options.Retry.ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .Handle<TimeoutRejectedException>()
                    .HandleResult(response => response.StatusCode == HttpStatusCode.BadGateway)
                    .HandleResult(response => response.StatusCode == HttpStatusCode.GatewayTimeout);
            });
    }
}