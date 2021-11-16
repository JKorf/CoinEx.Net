using System;
using CryptoExchange.Net.Interfaces;

namespace CoinEx.Net.Objects.Internal
{
    internal class CoinExNonceProvider : INonceProvider
    {
        private static readonly object nonceLock = new object();
        private static long? lastNonce;

        /// <inheritdoc />
        public long GetNonce()
        {
            lock (nonceLock)
            {
                var nonce = DateTime.UtcNow.Ticks;
                if (lastNonce.HasValue && nonce <= lastNonce.Value)
                    nonce = lastNonce.Value + 1;
                lastNonce = nonce;
                return nonce;
            }
        }
    }
}
