using CryptoExchange.Net.Interfaces;
using System;

namespace CoinEx.Net.Objects
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
                long nonce;
                if (lastNonce == null)
                    nonce = DateTime.UtcNow.Ticks;
                else
                    nonce = lastNonce.Value + 1;
                lastNonce = nonce;
                return nonce;
            }
        }
    }
}
