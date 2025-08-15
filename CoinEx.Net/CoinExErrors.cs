using CryptoExchange.Net.Objects.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinEx.Net
{
    internal static class CoinExErrors
    {
        internal static ErrorCollection RestErrorMapping { get; } = new ErrorCollection(
            [
                new ErrorInfo(ErrorType.Unauthorized, false, "Invalid API key", "4005"),
                new ErrorInfo(ErrorType.Unauthorized, false, "IP address not allowed", "4007"),
                new ErrorInfo(ErrorType.Unauthorized, false, "User not allowed", "4011"),
                new ErrorInfo(ErrorType.Unauthorized, false, "User not allowed to trade", "4018"),
                new ErrorInfo(ErrorType.Unauthorized, false, "Insufficient permissions", "4512"),
                new ErrorInfo(ErrorType.Unauthorized, false, "Signature error", "4006"),
                new ErrorInfo(ErrorType.Unauthorized, false, "Signature invalid", "4008"),

                new ErrorInfo(ErrorType.SystemError, true, "System busy, try again", "3008"),
                new ErrorInfo(ErrorType.SystemError, true, "Service unavailable, try again", "4001"),
                new ErrorInfo(ErrorType.SystemError, false, "Internal error", "4003"),

                new ErrorInfo(ErrorType.Timeout, true, "Service request timed out, try again", "4002"),

                new ErrorInfo(ErrorType.InvalidTimestamp, false, "Request expired", "227"),

                new ErrorInfo(ErrorType.Timeout, false, "Request expired", "4010"),
                new ErrorInfo(ErrorType.Timeout, false, "Signature expired", "4017"),

                new ErrorInfo(ErrorType.InsufficientBalance, false, "Insufficient balance", "3109"),

                new ErrorInfo(ErrorType.InvalidParameter, false, "Parameter error", "4004"),

                new ErrorInfo(ErrorType.InvalidQuantity, false, "Order quantity too low", "3127"),
                new ErrorInfo(ErrorType.InvalidQuantity, false, "The estimated ask price is lower than the current bottom ask price. Please reduce the quantity", "3612"),
                new ErrorInfo(ErrorType.InvalidQuantity, false, "The estimated bid price is higher than the current top bid price. Please reduce the quantity", "3613"),
                new ErrorInfo(ErrorType.InvalidQuantity, false, "The deviation between your estimated filled price and the index price is too big. Please reduce the quantity", "3614", "3634", "3635"),

                new ErrorInfo(ErrorType.InvalidPrice, false, "Order price deviates too much from current price", "3606"),
                new ErrorInfo(ErrorType.InvalidPrice, false, "The deviation between your order price and the index price is too high", "3615"),
                new ErrorInfo(ErrorType.InvalidPrice, false, "The order price exceeds the current top bid price", "3616", "3632"),
                new ErrorInfo(ErrorType.InvalidPrice, false, "The order price exceeds the current bottom ask price", "3617", "3633"),
                new ErrorInfo(ErrorType.InvalidPrice, false, "The deviation between your order price and the index price is too high", "3618"),
                new ErrorInfo(ErrorType.InvalidPrice, false, "The deviation between your order price and the trigger price is too high", "3619"),

                new ErrorInfo(ErrorType.UnavailableSymbol, false, "Symbol not currently trading", "4117", "4130"),
                new ErrorInfo(ErrorType.UnavailableSymbol, false, "Symbol not currently allowing API trading", "4159"),

                new ErrorInfo(ErrorType.RejectedOrderConfiguration, false, "Order cancellation not allowed during Call Auction", "3610"),
                new ErrorInfo(ErrorType.RejectedOrderConfiguration, false, "Market order submission is temporarily unavailable due to insufficient depth in the current market", "3620"),
                new ErrorInfo(ErrorType.RejectedOrderConfiguration, false, "This order can't be completely executed and has been canceled", "3621"),
                new ErrorInfo(ErrorType.RejectedOrderConfiguration, false, "This order can't be set as Maker Only and has been canceled", "3622"),
                new ErrorInfo(ErrorType.RejectedOrderConfiguration, false, "The current market depth is low, please reduce your order amount and try again", "3627", "3628", "3629"),
                new ErrorInfo(ErrorType.RejectedOrderConfiguration, false, "Currently in protection period, only Maker Only Limit Orders placement and order cancellations are supported", "3638"),

                new ErrorInfo(ErrorType.RateLimitRequest, false, "Too many requests", "4213"),
            ],
            [
                new ErrorEvaluator("3639", (code, msg) => {
                    if (msg?.Length > 0 && msg.StartsWith("Invalid Parameter: market ") && msg.EndsWith(" not found"))
                        return new ErrorInfo(ErrorType.UnknownSymbol, false, "Unknown symbol", code);

                     return new ErrorInfo(ErrorType.InvalidParameter, false, "Parameter error", "3639", code);
                })
            ]
        );

        internal static ErrorCollection SocketErrorMapping { get; } = new ErrorCollection(
            [
                new ErrorInfo(ErrorType.Unauthorized, false, "Authentication failed", "21002"),

                new ErrorInfo(ErrorType.InvalidParameter, false, "Invalid parameter", "20001"),
                new ErrorInfo(ErrorType.InvalidParameter, false, "Requested method not found", "20002"),

                new ErrorInfo(ErrorType.Timeout, false, "Request timeout", "23001"),

                new ErrorInfo(ErrorType.RateLimitRequest, false, "Too many requests", "23002"),

                new ErrorInfo(ErrorType.SystemError, false, "Internal error", "24001"),
                new ErrorInfo(ErrorType.SystemError, true, "Service temporarily unavailable", "24002")
            ]
        );
    }
}