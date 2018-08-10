using CryptoExchange.Net.Authentication;
using System;
using System.Security.Cryptography;
using System.Text;

namespace CoinEx.Net
{
    internal class CoinExAuthenticationProvider: AuthenticationProvider
    {
        private readonly MD5 encryptor;


        public CoinExAuthenticationProvider(ApiCredentials credentials): base(credentials)
        {
            encryptor = MD5.Create();
        }

        public override string Sign(string toSign)
        {
            return BitConverter.ToString(encryptor.ComputeHash(Encoding.ASCII.GetBytes(toSign))).Replace("-", string.Empty).ToUpper();
        }
    }
}
