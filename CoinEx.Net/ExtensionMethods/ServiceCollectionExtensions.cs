using CoinEx.Net;
using CoinEx.Net.Clients;
using CoinEx.Net.Interfaces;
using CoinEx.Net.Interfaces.Clients;
using CoinEx.Net.Objects.Options;
using CoinEx.Net.SymbolOrderBooks;
using CryptoExchange.Net;
using CryptoExchange.Net.Clients;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Interfaces.Clients;
using CryptoExchange.Net.SharedApis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extensions for DI
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add services such as the ICoinExRestClient and ICoinExSocketClient. Configures the services based on the provided configuration.<br />
        /// See <see href="https://github.com/JKorf/CoinEx.Net/blob/master/Examples/example-config.json" /> for an example of how to set up the configuration.
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configuration">The configuration(section) containing the options</param>
        /// <returns></returns>
        public static IServiceCollection AddCoinEx(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var options = CoinExOptions.CreateFromConfiguration(configuration);
            services.AddSingleton(Options.Options.Create(options.Rest));
            services.AddSingleton(Options.Options.Create(options.Socket));
            services.AddSingleton(Options.Options.Create(options));

            return AddCoinExCore(services, options.SocketClientLifeTime);
        }

        /// <summary>
        /// Add services such as the ICoinExRestClient and ICoinExSocketClient. Services will be configured based on the provided options.
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="optionsDelegate">Set options for the CoinEx services</param>
        /// <returns></returns>
        public static IServiceCollection AddCoinEx(
            this IServiceCollection services,
            Action<CoinExOptions>? optionsDelegate = null)
        {
            var options = CoinExOptions.Create(optionsDelegate);
            services.AddSingleton(Options.Options.Create(options.Rest));
            services.AddSingleton(Options.Options.Create(options.Socket));
            services.AddSingleton(Options.Options.Create(options));

            return AddCoinExCore(services, options.SocketClientLifeTime);
        }

        private static IServiceCollection AddCoinExCore(
            this IServiceCollection services,
            ServiceLifetime? socketClientLifeTime = null)
        {
            services.AddHttpClient<ICoinExRestClient, CoinExRestClient>((client, serviceProvider) =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<CoinExRestOptions>>().Value;
                client.Timeout = options.RequestTimeout;
                return new CoinExRestClient(client, serviceProvider.GetRequiredService<ILoggerFactory>(), serviceProvider.GetRequiredService<IOptions<CoinExRestOptions>>());
            }).ConfigurePrimaryHttpMessageHandler((serviceProvider) => {
                var options = serviceProvider.GetRequiredService<IOptions<CoinExRestOptions>>().Value;
                return LibraryHelpers.CreateHttpClientMessageHandler(options);
            }).SetHandlerLifetime(Timeout.InfiniteTimeSpan);
            services.Add(new ServiceDescriptor(typeof(ICoinExSocketClient), x => { return new CoinExSocketClient(x.GetRequiredService<IOptions<CoinExSocketOptions>>(), x.GetRequiredService<ILoggerFactory>()); }, socketClientLifeTime ?? ServiceLifetime.Singleton));

            services.AddTransient<ICoinExOrderBookFactory, CoinExOrderBookFactory>();
            services.AddTransient<ICoinExTrackerFactory, CoinExTrackerFactory>();
            services.AddTransient<ITrackerFactory, CoinExTrackerFactory>();
            services.AddSingleton<ICoinExUserClientProvider, CoinExUserClientProvider>(x =>
            new CoinExUserClientProvider(
                x.GetRequiredService<IHttpClientFactory>().CreateClient(typeof(ICoinExRestClient).Name),
                x.GetRequiredService<ILoggerFactory>(),
                x.GetRequiredService<IOptions<CoinExRestOptions>>(),
                x.GetRequiredService<IOptions<CoinExSocketOptions>>()));
                        
            services.RegisterSharedRestInterfaces(x => x.GetRequiredService<ICoinExRestClient>().SpotApiV2.SharedClient);
            services.RegisterSharedSocketInterfaces(x => x.GetRequiredService<ICoinExSocketClient>().SpotApiV2.SharedClient);
            services.RegisterSharedRestInterfaces(x => x.GetRequiredService<ICoinExRestClient>().FuturesApi.SharedClient);
            services.RegisterSharedSocketInterfaces(x => x.GetRequiredService<ICoinExSocketClient>().FuturesApi.SharedClient);

            services.RegisterSharedApiClient<
                ICoinExSharedApiClient,
                CoinExSharedApiClient>(sharedApis => sharedApis
                    .Add(client => client.SpotRest)
                    .Add(client => client.SpotSocket)
                    .Add(client => client.FuturesRest)
                    .Add(client => client.FuturesSocket)
                    );

            return services;
        }
    }
}
