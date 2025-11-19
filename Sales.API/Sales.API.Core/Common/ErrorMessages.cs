namespace Sales.API.Core.Common
{
    public static class ErrorMessages
    {
        public const string INVALIDSALESTATUSRESPONSE = "Invalid SaleStatus response received.";

        public const string INVALIDSALEREQUEST = "Product or Buyer not found.";

        public const string INCORRECTFORMAT = "Incorrect format.";

        public const string SALENOTFOUND = "Sale not found.";

        public const string NOSALESFOUND = "No sales are available.";

        public const string SALEFAIL = "Sale could not be completed: {error}";
    }
}
