namespace Sales.API.Core.Enum
{
    public enum ErrorType
    {
        Undefined = 0, 

        NotFound = 2, 

        InternalError = 3, 

        DatabaseError = 4, 

        IntegrationError = 5, 

        BusinessRuleViolation = 6
    }
}
