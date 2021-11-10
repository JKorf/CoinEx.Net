using CryptoExchange.Net.Interfaces;
using System;

namespace CoinEx.Net.Objects
{
    internal class CoinExNonceProvider : INonceProvider
    {
        private static DateTime unix = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly object nonceLock = new object();
        private static long? lastNonce;

        /// <inheritdoc />
        public long GetNonce()
        {
            lock (nonceLock)
            {
                var nonce = (long)Math.Round((DateTime.UtcNow - unix).TotalMilliseconds);
                if (lastNonce.HasValue && nonce <= lastNonce.Value)
                    nonce = lastNonce.Value + 1;
                lastNonce = nonce;
                return nonce;
            }
        }
    }
}
