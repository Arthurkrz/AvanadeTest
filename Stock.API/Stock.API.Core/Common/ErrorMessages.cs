namespace Stock.API.Core.Common
{
    public static class ErrorMessages
    {
        public const string PRODUCTNOTFOUND = "Product not found.";

        public const string DATABASEERROR = "An unexpected database error occurred: {error}";

        public const string INTERNALERROR = "An unexpected internal error occurred: {error}";

        public const string INVALIDREQUEST = "The request is invalid: {error}";

        public const string NOPRODUCTSFOUND = "No products available.";

        public const string INCORRECTFORMAT = "Incorrect format.";
    }
}
