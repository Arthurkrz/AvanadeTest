using Sales.API.Core.Enum;

namespace Sales.API.Core.Common
{
    public class SaleApiException : Exception
    {
        public SaleApiException (string error, ErrorType errorType) : base(error)
        {
            Error = error;
            ErrorType = errorType;
        }

        public string Error { get; }
        public ErrorType ErrorType { get; }
    }
}
