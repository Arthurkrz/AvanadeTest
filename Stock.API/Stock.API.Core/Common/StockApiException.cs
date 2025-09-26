using Stock.API.Core.Enum;

namespace Stock.API.Core.Common
{
    public class StockApiException : Exception
    {
        public StockApiException(string error, ErrorType errorType)
        {
            Error = error;
            ErrorType = errorType;
        }

        public string Error { get; }
        public ErrorType ErrorType { get; }
    }
}
