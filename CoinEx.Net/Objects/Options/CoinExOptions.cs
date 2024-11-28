using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Objects.Options;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoinEx.Net.Objects.Options
{
    /// <summary>
    /// CoinEx options
    /// </summary>
    public class CoinExOptions : LibraryOptions<CoinExRestOptions, CoinExSocketOptions, ApiCredentials, CoinExEnvironment>
    {
    }
}
