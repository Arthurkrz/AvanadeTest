using FluentValidation;
using Stock.API.Web.DTOs;

namespace Stock.API.Web.Validators
{
    public class ProductDTOValidator : AbstractValidator<ProductDTO>
    {
        public ProductDTOValidator()
        {
            this.RuleFor(p => p.Name)                 
                .NotEmpty()
                .WithMessage("Product name must not be null or empty");

            this.RuleFor(p => p.Description)
                .NotEmpty()
                .WithMessage("Product description must not be null or empty");

            this.RuleFor(p => p.Price)
                .GreaterThan(0)
                .WithMessage("Product price must not be zero or negative");

            this.RuleFor(p => p.AmountInStock)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Product amount must not be negative");
        }
    }
}
