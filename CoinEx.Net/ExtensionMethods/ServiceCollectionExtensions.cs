using CoinEx.Net;
using CoinEx.Net.Clients;
using CoinEx.Net.Interfaces;
using CoinEx.Net.Interfaces.Clients;
using CoinEx.Net.Objects.Options;
using CoinEx.Net.SymbolOrderBooks;
using CryptoExchange.Net;
using CryptoExchange.Net.Clients;
using CryptoExchange.Net.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Http;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extensions for DI
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add services such as the ICoinExRestClient and ICoinExSocketClient. Configures the services based on the provided configuration.
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configuration">The configuration(section) containing the options</param>
        /// <returns></returns>
        public static IServiceCollection AddCoinEx(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var options = new CoinExOptions();
            // Reset environment so we know if they're overridden
            options.Rest.Environment = null!;
            options.Socket.Environment = null!;
            configuration.Bind(options);

            if (options.Rest == null || options.Socket == null)
                throw new ArgumentException("Options null");

            var restEnvName = options.Rest.Environment?.Name ?? options.Environment?.Name ?? CoinExEnvironment.Live.Name;
            var socketEnvName = options.Socket.Environment?.Name ?? options.Environment?.Name ?? CoinExEnvironment.Live.Name;
            options.Rest.Environment = CoinExEnvironment.GetEnvironmentByName(restEnvName) ?? options.Rest.Environment!;
            options.Rest.ApiCredentials = options.Rest.ApiCredentials ?? options.ApiCredentials;
            options.Rest.AllowAppendingClientOrderId = options.Rest.AllowAppendingClientOrderId || options.AllowAppendingClientOrderId;
            options.Socket.Environment = CoinExEnvironment.GetEnvironmentByName(socketEnvName) ?? options.Socket.Environment!;
            options.Socket.ApiCredentials = options.Socket.ApiCredentials ?? options.ApiCredentials;

            services.AddSingleton(x => Options.Options.Create(options.Rest));
            services.AddSingleton(x => Options.Options.Create(options.Socket));

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
            var options = new CoinExOptions();
            // Reset environment so we know if they're overridden
            options.Rest.Environment = null!;
            options.Socket.Environment = null!;
            optionsDelegate?.Invoke(options);
            if (options.Rest == null || options.Socket == null)
                throw new ArgumentException("Options null");

            options.Rest.AllowAppendingClientOrderId = options.Rest.AllowAppendingClientOrderId || options.AllowAppendingClientOrderId;
            options.Rest.Environment = options.Rest.Environment ?? options.Environment ?? CoinExEnvironment.Live;
            options.Rest.ApiCredentials = options.Rest.ApiCredentials ?? options.ApiCredentials;
            options.Socket.Environment = options.Socket.Environment ?? options.Environment ?? CoinExEnvironment.Live;
            options.Socket.ApiCredentials = options.Socket.ApiCredentials ?? options.ApiCredentials;

            services.AddSingleton(x => Options.Options.Create(options.Rest));
            services.AddSingleton(x => Options.Options.Create(options.Socket));

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
                var handler = new HttpClientHandler();
                try
                {
                    handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                    handler.DefaultProxyCredentials = CredentialCache.DefaultCredentials;
                }
                catch (PlatformNotSupportedException) { }
                catch (NotImplementedException) { } // Mono runtime throws NotImplementedException for DefaultProxyCredentials setting

                var options = serviceProvider.GetRequiredService<IOptions<CoinExRestOptions>>().Value;
                if (options.Proxy != null)
                {
                    handler.Proxy = new WebProxy
                    {
                        Address = new Uri($"{options.Proxy.Host}:{options.Proxy.Port}"),
                        Credentials = options.Proxy.Password == null ? null : new NetworkCredential(options.Proxy.Login, options.Proxy.Password)
                    };
                }
                return handler;
            });
            services.Add(new ServiceDescriptor(typeof(ICoinExSocketClient), x => { return new CoinExSocketClient(x.GetRequiredService<IOptions<CoinExSocketOptions>>(), x.GetRequiredService<ILoggerFactory>()); }, socketClientLifeTime ?? ServiceLifetime.Singleton));

            services.AddTransient<ICryptoRestClient, CryptoRestClient>();
            services.AddTransient<ICryptoSocketClient, CryptoSocketClient>();
            services.AddTransient<ICoinExOrderBookFactory, CoinExOrderBookFactory>();
            services.AddTransient<ICoinExTrackerFactory, CoinExTrackerFactory>();

            services.RegisterSharedRestInterfaces(x => x.GetRequiredService<ICoinExRestClient>().SpotApiV2.SharedClient);
            services.RegisterSharedSocketInterfaces(x => x.GetRequiredService<ICoinExSocketClient>().SpotApiV2.SharedClient);
            services.RegisterSharedRestInterfaces(x => x.GetRequiredService<ICoinExRestClient>().FuturesApi.SharedClient);
            services.RegisterSharedSocketInterfaces(x => x.GetRequiredService<ICoinExSocketClient>().FuturesApi.SharedClient);

            return services;
        }
    }
}
