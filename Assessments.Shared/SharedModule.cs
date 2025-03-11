using Assessments.Shared.Interfaces;
using LazyCache;
using Microsoft.Extensions.DependencyInjection;
using System;
using Assessments.Shared.Repositories;

namespace Assessments.Shared;

public static class SharedModule
{
    public static void AddCoreApplication(this IServiceCollection services)
    {
        services.AddLazyCache(_ => {
            var cache = new CachingService(CachingService.DefaultCacheProvider)
            {
                DefaultCachePolicy =
                {
                    DefaultCacheDurationSeconds = (int) TimeSpan.FromHours(1).TotalSeconds
                }
            };
            return cache;
        });

        services.AddSingleton<INatureTypesRepository, NatureTypesRepository>();
    }
}