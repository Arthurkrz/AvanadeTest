namespace Identity.API.Core.Common
{
    public class ErrorMessages
    {
        public const string ADMINNOTFOUND = "Admin not found.";

        public const string LOCKEDACCOUNT = "Account locked until {lockoutEnd}.";

        public const string INVALIDCREDENTIALS = "Invalid username and/or password.";

        public const string EMPTYCREDENTIALS = "Empty username and/or password.";

        public const string ADMINNOTUNIQUE = "Admin with a unique field (Username, Name or CPF) already exists.";

        public const string INVALIDREQUEST = "The request is invalid: {error}";

        public const string INCORRECTFORMAT = "Incorrect format.";
    }
}
