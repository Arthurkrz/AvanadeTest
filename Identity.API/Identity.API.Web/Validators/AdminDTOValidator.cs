using Identity.API.Web.DTOs;

namespace Identity.API.Web.Validators
{
    public class AdminDTOValidator : AbstractValidator<AdminDTO>
    {
        public AdminDTOValidator()
        {
            this.RuleFor(p => p.Username)
                .NotEmpty()
                .WithMessage("Username must not be null or empty");

            this.RuleFor(p => p.Name)
                .NotEmpty()
                .WithMessage("Name must not be null or empty");

            this.RuleFor(p => p.CPF)
                .NotEmpty()
                .WithMessage("CPF must not be null or empty");

            this.RuleFor(p => p.Password)
                .NotEmpty()
                .WithMessage("Password must not be null or empty");
        }
    }
}
