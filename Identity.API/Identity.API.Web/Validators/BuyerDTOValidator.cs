using FluentValidation;
using Identity.API.Web.DTOs;

namespace Identity.API.Web.Validators
{
    public class BuyerDTOValidator : AbstractValidator<BuyerDTO>
    {
        public BuyerDTOValidator()
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
            
            this.RuleFor(p => p.Email)
                .NotEmpty()
                .WithMessage("Email must not be null or empty");
            
            this.RuleFor(p => p.PhoneNumber)
                .NotEmpty()
                .WithMessage("Phone Number must not be null or empty");
            
            this.RuleFor(p => p.DeliveryAddress)
                .NotEmpty()
                .WithMessage("Delivery Address must not be null or empty");
        }
    }
}
