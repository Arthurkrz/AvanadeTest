using Stock.API.Core.Entities;
using Stock.API.Core.Enum;

namespace Stock.API.Core.Common
{
    public class Response<T> where T : Entity
    {
        private readonly T? _value;

        public T? Value
        {
            get
            {
                if (!Success) throw new InvalidOperationException("Cannot access Value when the result is not successful.");

                return _value;
            }
        }

        public ErrorType? ErrorType { get; set; }

        public IList<string> Errors { get; }

        public bool Success { get; }

        private Response(T? value, ErrorType? errorType, IList<string>? errors, bool success)
        {
            _value = value;
            ErrorType = errorType;
            Errors = errors ?? new List<string>();
            Success = success;
        }

        public static Response<T> Ok(T value) =>
            new Response<T>(value, null, null, true);

        public static Response<T> Ko(ErrorType errorType, IList<string> errors)
        {
            if (errorType == 0) 
                throw new ArgumentException("ErrorType cannot be undefined.", nameof(errorType));

            if (errors == null || errors.Count == 0)
                throw new ArgumentException("Errors cannot be null or empty.", nameof(errors));

            return new Response<T>(null, errorType, errors, false);
        }
    }
}
