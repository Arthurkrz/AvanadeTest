using Identity.API.Core.Enum;

namespace Identity.API.Core.Common
{
    public class IdentityApiException : Exception
    {
        public IdentityApiException(string error, ErrorType errorType) : base(error)
        {
            Error = error;
            ErrorType = errorType;
        }

        public string Error { get; }
        public ErrorType ErrorType { get; }
    }
}
