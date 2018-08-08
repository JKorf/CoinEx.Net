using System;
using System.Collections.Generic;
using System.Text;

namespace CoinEx.Net.Objects.Websocket
{
    public class CoinExSocketError
    {
        public int Code { get; set; }
        public string Message { get; set; }
    }
}
