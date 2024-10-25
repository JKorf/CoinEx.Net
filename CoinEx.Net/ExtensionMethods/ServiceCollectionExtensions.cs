using CoinEx.Net;
using CoinEx.Net.Clients;
using CoinEx.Net.Interfaces;
using CoinEx.Net.Interfaces.Clients;
using CoinEx.Net.Objects.Options;
using CoinEx.Net.SymbolOrderBooks;
using CryptoExchange.Net;
using CryptoExchange.Net.Clients;
using CryptoExchange.Net.Interfaces;
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
        /// Add the ICoinExClient and ICoinExSocketClient to the sevice collection so they can be injected
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="defaultRestOptionsDelegate">Set default options for the rest client</param>
        /// <param name="defaultSocketOptionsDelegate">Set default options for the socket client</param>
        /// <param name="socketClientLifeTime">The lifetime of the ICoinExSocketClient for the service collection. Defaults to Singleton.</param>
        /// <returns></returns>
        public static IServiceCollection AddCoinEx(
            this IServiceCollection services,
            Action<CoinExRestOptions>? defaultRestOptionsDelegate = null,
            Action<CoinExSocketOptions>? defaultSocketOptionsDelegate = null,
            ServiceLifetime? socketClientLifeTime = null)
        {
            var restOptions = CoinExRestOptions.Default.Copy();

            if (defaultRestOptionsDelegate != null)
            {
                defaultRestOptionsDelegate(restOptions);
                CoinExRestClient.SetDefaultOptions(defaultRestOptionsDelegate);
            }

            if (defaultSocketOptionsDelegate != null)
                CoinExSocketClient.SetDefaultOptions(defaultSocketOptionsDelegate);

            services.AddHttpClient<ICoinExRestClient, CoinExRestClient>(options =>
            {
                options.Timeout = restOptions.RequestTimeout;
            }).ConfigurePrimaryHttpMessageHandler(() => {
                var handler = new HttpClientHandler();
                if (restOptions.Proxy != null)
                {
                    handler.Proxy = new WebProxy
                    {
                        Address = new Uri($"{restOptions.Proxy.Host}:{restOptions.Proxy.Port}"),
                        Credentials = restOptions.Proxy.Password == null ? null : new NetworkCredential(restOptions.Proxy.Login, restOptions.Proxy.Password)
                    };
                }
                return handler;
            });

            services.AddTransient<ICryptoRestClient, CryptoRestClient>();
            services.AddTransient<ICryptoSocketClient, CryptoSocketClient>();
            services.AddTransient<ICoinExOrderBookFactory, CoinExOrderBookFactory>();
            services.AddTransient<ICoinExTrackerFactory, CoinExTrackerFactory>();
            services.AddTransient(x => x.GetRequiredService<ICoinExRestClient>().SpotApiV2.CommonSpotClient);

            services.RegisterSharedRestInterfaces(x => x.GetRequiredService<ICoinExRestClient>().SpotApiV2.SharedClient);
            services.RegisterSharedSocketInterfaces(x => x.GetRequiredService<ICoinExSocketClient>().SpotApiV2.SharedClient);
            services.RegisterSharedRestInterfaces(x => x.GetRequiredService<ICoinExRestClient>().FuturesApi.SharedClient);
            services.RegisterSharedSocketInterfaces(x => x.GetRequiredService<ICoinExSocketClient>().FuturesApi.SharedClient);

            if (socketClientLifeTime == null)
                services.AddSingleton<ICoinExSocketClient, CoinExSocketClient>();
            else
                services.Add(new ServiceDescriptor(typeof(ICoinExSocketClient), typeof(CoinExSocketClient), socketClientLifeTime.Value));
            return services;
        }
    }
}
