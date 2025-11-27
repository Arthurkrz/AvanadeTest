namespace Stock.API.Core.Enum
{
    public enum ErrorType
    {
        Undefined = 0,
        
        NotFound = 1,
        
        InternalError = 2,
        
        DatabaseError = 3,
        
        IntegrationError = 4,

        BusinessRuleViolation = 5
    }
}
