using System;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.ServiceFabric;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceFabricCacheServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Service Fabric reliable collection <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="setupAction">An <see cref="Action{DynamoDbCacheOptions}"/> to configure the provided
        /// <see cref="ServiceFabricCacheOptions"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>                    
        public static IServiceCollection AddDistributedServiceFabricCache(this IServiceCollection services, Action<ServiceFabricCacheOptions> setupAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            services.AddOptions();
            services.Configure(setupAction);
            services.Add(ServiceDescriptor.Singleton<IDistributedCache, ServiceFabricCache>());

            return services;
        }
    }
}
