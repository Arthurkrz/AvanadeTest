using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Stock.API.Core.Entities;
using Stock.API.Core.Validators;

namespace Stock.API.Tests.Validators
{
    public class ProductValidatorTests
    {
        private readonly IValidator<Product> _sut;

        public ProductValidatorTests()
        {
            _sut = new ProductValidator();
        }

        [Theory]
        [MemberData(nameof(GetInvalidProducts))]
        public void Validate_ShouldReturnErrors(Product product, List<string> errors)
        {
            // Act & Assert
            var result = _sut.Validate(product);

            Assert.False(result.IsValid);

            result.Errors.Select(e => e.ErrorMessage.Trim()).ToList()
                         .Should().BeEquivalentTo(errors);
        }

        [Fact]
        public void Validate_ShouldReturnSuccess()
        {
            // Arrange
            var product = new Product("Valid Name", "Valid Description", 999999, 999999);

            // Act & Assert
            Assert.True(_sut.Validate(product).IsValid);
        }

        public static IEnumerable<object[]> GetInvalidProducts()
        {
            yield return new object[]
            {
                new Product("", "Description", 10, 10),

                new List<string>
                {
                    "Product name must not be null or empty"
                }
            };

            yield return new object[]
            {
                new Product(new string('a', 101), "Description", 10, 10),

                new List<string>
                {
                    "Product name must not exceed 100 characters"
                }
            };

            yield return new object[]
            {
                new Product("Name", "", 10, 10),

                new List<string>
                {
                    "Product description must not be null or empty"
                }
            };

            yield return new object[]
            {
                new Product("Name", new string('a', 501), 10, 10),

                new List<string>
                {
                    "Product description must not exceed 500 characters"
                }
            };

            yield return new object[]
            {
                new Product("Name", "Description", 1000000, 10),

                new List<string>
                {
                    "Price cannot be equal or exceed 1.000.000"
                }
            };

            yield return new object[]
            {
                new Product("Name", "Description", 1000001, 10),

                new List<string>
                {
                    "Price cannot be equal or exceed 1.000.000"
                }
            };

            yield return new object[]
            {
                new Product("Name", "Description", 0, 10),

                new List<string>
                {
                    "Price cannot be zero or negative"
                }
            };

            yield return new object[]
            {
                new Product("Name", "Description", -1, 10),

                new List<string>
                {
                    "Price cannot be zero or negative"
                }
            };

            yield return new object[]
            {
                new Product("Name", "Description", 10, 1000000),

                new List<string>
                {
                    "Amount in stock cannot be equal or exceed 1.000.000"
                }
            };

            yield return new object[]
            {
                new Product("Name", "Description", 10, 1000001),

                new List<string>
                {
                    "Amount in stock cannot be equal or exceed 1.000.000"
                }
            };

            yield return new object[]
            {
                new Product("Name", "Description", 10, -10),

                new List<string>
                {
                    "Amount in stock cannot be negative"
                }
            };

            yield return new object[]
            {
                new Product("", "", 0, -1),

                new List<string>
                {
                    "Product name must not be null or empty",
                    "Product description must not be null or empty",
                    "Price cannot be zero or negative",
                    "Amount in stock cannot be negative"
                }
            };

            yield return new object[]
            {
                new Product(new string('a', 101), new string('a', 501), 1000001, 1000001),

                new List<string>
                {
                    "Product name must not exceed 100 characters",
                    "Product description must not exceed 500 characters",
                    "Price cannot be equal or exceed 1.000.000",
                    "Amount in stock cannot be equal or exceed 1.000.000"
                }
            };
        }
    }
}
