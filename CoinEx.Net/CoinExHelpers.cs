using CoinEx.Net.Clients;
using CoinEx.Net.Enums;
using CoinEx.Net.Interfaces.Clients;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Net;
using System.Text.RegularExpressions;
using CoinEx.Net.Objects.Options;
using CoinEx.Net.SymbolOrderBooks;
using CoinEx.Net.Interfaces;
using CryptoExchange.Net.Clients;

namespace CoinEx.Net
{
    /// <summary>
    /// CoinEx helpers
    /// </summary>
    public static class CoinExHelpers
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

            services.AddTransient<ICryptoExchangeClient, CryptoExchangeClient>();
            services.AddSingleton<ICoinExOrderBookFactory, CoinExOrderBookFactory>();
            services.AddTransient(x => x.GetRequiredService<ICoinExRestClient>().SpotApi.CommonSpotClient);
            if (socketClientLifeTime == null)
                services.AddSingleton<ICoinExSocketClient, CoinExSocketClient>();
            else
                services.Add(new ServiceDescriptor(typeof(ICoinExSocketClient), typeof(CoinExSocketClient), socketClientLifeTime.Value));
            return services;
        }

        /// <summary>
        /// Kline interval to seconds
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static int ToSeconds(this KlineInterval interval)
        {
            return interval switch
            {
                KlineInterval.OneMinute => 1 * 60,
                KlineInterval.ThreeMinutes => 3 * 60,
                KlineInterval.FiveMinutes => 5 * 60,
                KlineInterval.FifteenMinutes => 15 * 60,
                KlineInterval.ThirtyMinutes => 30 * 60,
                KlineInterval.OneHour => 1 * 60 * 60,
                KlineInterval.TwoHours => 2 * 60 * 60,
                KlineInterval.FourHours => 4 * 60 * 60,
                KlineInterval.SixHours => 6 * 60 * 60,
                KlineInterval.TwelveHours => 12 * 60 * 60,
                KlineInterval.OneDay => 1 * 24 * 60 * 60,
                KlineInterval.ThreeDays => 3 * 24 * 60 * 60,
                KlineInterval.OneWeek => 7 * 24 * 60 * 60,
                _ => 0,
            };
        }

        /// <summary>
        /// Merge depth to string parameter
        /// </summary>
        /// <param name="depth"></param>
        /// <returns></returns>
        public static string MergeDepthIntToString(int depth)
        {
            var merge = "0";
            if (depth == 8)
                return merge;

            merge += ".";
            for (var i = 0; i < 7 - depth; i++)
                merge += "0";
            merge += "1";
            return merge;
        }

        /// <summary>
        /// Validate the string is a valid CoinEx symbol.
        /// </summary>
        /// <param name="symbolString">string to validate</param>
        public static void ValidateCoinExSymbol(this string symbolString)
        {
            if (string.IsNullOrEmpty(symbolString))
                throw new ArgumentException("Symbol is not provided");

            if (!Regex.IsMatch(symbolString, "^([0-9A-Z]{5,})$"))
                throw new ArgumentException($"{symbolString} is not a valid CoinEx symbol. Should be [BaseAsset][QuoteAsset], e.g. ETHBTC");
        }
    }
}
