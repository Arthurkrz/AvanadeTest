using FluentValidation;
using Identity.API.Core.Common;

namespace Identity.API.Core.Validators
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
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
        }

        private bool IsNameValid(string name)
        {
            var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length >= 2 && parts.All(p => p.Length >= 3);
        }
    }
}
