using FluentValidation;
using Stock.API.Web.DTOs;

namespace Stock.API.Web.Validators
{
    public class AdminDTOValidator : AbstractValidator<AdminDTO>
    {
        public AdminDTOValidator()
        {
            this.RuleFor(p => p.Username)
                .NotNull()
                .WithMessage("Username must not be null")
                .NotEmpty()
                .WithMessage("Username must not be empty");

            this.RuleFor(p => p.Name)
                .NotNull()
                .WithMessage("Name must not be null")
                .NotEmpty()
                .WithMessage("Name must not be empty");

            this.RuleFor(p => p.CPF)
                .NotNull()
                .WithMessage("CPF must not be null")
                .NotEmpty()
                .WithMessage("CPF must not be empty");

            this.RuleFor(p => p.Password)
                .NotNull()
                .WithMessage("Password must not be null")
                .NotEmpty()
                .WithMessage("Password must not be empty");
        }
    }
}
