using FluentValidation;
using Identity.API.Core.Common;

namespace Identity.API.Core.Validators
{
    public class UserRegisterRequestValidator : AbstractValidator<UserRegisterRequest>
    {
        public UserRegisterRequestValidator()
        {
            this.RuleFor(p => p.Username)
                .MinimumLength(3)
                .WithMessage("Username must have at least 3 characters");

            this.RuleFor(p => p.Name)
                .Must(IsNameValid)
                .WithMessage("Name must contain name and surname separated by space with at least 3 characters in each");

            this.RuleFor(p => p.CPF)
                .Length(11)
                .WithMessage("CPF number must contain exactly 11 digits")
                .Matches("^[0-9]+$")
                .WithMessage("CPF number must contain only digits");

            this.RuleFor(p => p.Password)
                .MinimumLength(8)
                .WithMessage("Password must be at least 8 characters long")
                .Matches("[A-Z]")
                .WithMessage("Password must contain at least one uppercase letter")
                .Matches("[^a-zA-z0-9]")
                .WithMessage("Password must contain at least one special character");

            this.RuleFor(p => p.Email)
                .EmailAddress()
                .WithMessage("Email must be a valid email address");
            
            this.RuleFor(p => p.PhoneNumber)
                .Matches(@"^[^a-zA-Z]*$")
                .WithMessage("Phone Number must not contain letters")
                .MinimumLength(11)
                .WithMessage("Phone Number must contain at least 11 digits")
                .MaximumLength(15)
                .WithMessage("Phone Number must not contain more than 15 digits");

            this.RuleFor(p => p.DeliveryAddress)
                .Matches(@"\d")
                .WithMessage("Delivery Address must contain a street number")
                .MinimumLength(5)
                .WithMessage("Delivery Address must be at least 5 characters long");
        }

        private bool IsNameValid(string name)
        {
            var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length >= 2 && parts.All(p => p.Length >= 3);
        }
    }
}
