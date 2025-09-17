using FluentValidation;
using Stock.API.Web.DTOs;

namespace Stock.API.Web.Validators
{
    public class ProductDTOValidator : AbstractValidator<ProductDTO>
    {
        public ProductDTOValidator()
        {
            this.RuleFor(p => p.Name)
                .NotNull()
                .WithMessage("Product name must not be null.")                     
                .NotEmpty()
                .WithMessage("Product name must not be empty.");

            this.RuleFor(p => p.Description)
                .NotNull()
                .WithMessage("Product description must not be null.")
                .NotEmpty()
                .WithMessage("Product description must not be empty.");

            this.RuleFor(p => p.Price)
                .NotNull()
                .WithMessage("Product price must not be null.");

            this.RuleFor(p => p.AmountInStock)
                .NotNull()
                .WithMessage("Product amount in stock must not be null.");
        }
    }
}
