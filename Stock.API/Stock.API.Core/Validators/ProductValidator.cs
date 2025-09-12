using FluentValidation;
using Stock.API.Core.Entities;

namespace Stock.API.Core.Validators
{
    public class ProductValidator : AbstractValidator<Product>
    {
        public ProductValidator()
        {
            this.RuleFor(p => p.Name)
                .Must(n => !string.IsNullOrEmpty(n))
                .WithMessage("Product name must not be null or empty.");

            this.RuleFor(p => p.Name)
                .MaximumLength(100)
                .WithMessage("Product name must not exceed 100 characters.");

            this.RuleFor(p => p.Description)
                .Must(d => !string.IsNullOrEmpty(d))
                .WithMessage("Product description must not be null or empty.");

            this.RuleFor(p => p.Description)
                .MaximumLength(500)
                .WithMessage("Product description must not exceed 500 characters.");

            this.RuleFor(p => p.Price)
                .GreaterThan(0)
                .WithMessage("Price cannot be zero.");

            this.RuleFor(p => p.Price)
                .LessThan(1000000)
                .WithMessage("Price cannot be equal or exceed 1,000,000.");

            this.RuleFor(p => p.Price)
                .GreaterThan(0)
                .WithMessage("Price cannot be zero or negative.");

            this.RuleFor(p => p.AmountInStock)
                .LessThan(1000000)
                .WithMessage("Amount in stock cannot be equal or exced 1,000,000.");
        }
    }
}
