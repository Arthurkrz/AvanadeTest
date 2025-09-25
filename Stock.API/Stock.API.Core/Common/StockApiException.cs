using Stock.API.Core.Enum;

namespace Stock.API.Core.Common
{
    public class StockApiException : Exception
    {
        public StockApiException(List<string> errors, ErrorType errorType)
        {
            Errors = errors;
            ErrorType = errorType;
        }

        public List<string> Errors { get; }
        public ErrorType ErrorType { get; }
    }
}
