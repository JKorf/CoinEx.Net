using System;
using System.Collections.Generic;
using System.Text;

namespace CoinEx.Net.Objects.Websocket
{
    public class CoinExSocketRequestResponse<T>
    {
        public CoinExSocketError Error { get; set; }
        public T Result { get; set; }
        public int Id { get; set; }
    }
}
