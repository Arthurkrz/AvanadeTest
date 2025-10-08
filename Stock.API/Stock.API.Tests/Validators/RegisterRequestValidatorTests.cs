using FluentAssertions;
using FluentValidation;
using Stock.API.Core.Common;
using Stock.API.Core.Validators;

namespace Stock.API.Tests.Validators
{
    public class RegisterRequestValidatorTests
    {
        private readonly IValidator<RegisterRequest> _sut;

        public RegisterRequestValidatorTests()
        {
            _sut = new RegisterRequestValidator();
        }

        [Theory]
        [MemberData(nameof(GetInvalidRequests))]
        public void Validate_ShouldReturnErrors(RegisterRequest request, List<string> errors)
        {
            // Act & Assert
            var result = _sut.Validate(request);

            Assert.False(result.IsValid);

            result.Errors.Select(e => e.ErrorMessage.Trim()).ToList()
                         .Should().BeEquivalentTo(errors);
        }

        [Fact]
        public void Validate_ShouldReturnSuccess()
        {
            // Arrange
            var request = new RegisterRequest("Username", "Name Surname", "12345678910", "987Pjhmk?");

            // Act & Assert
            Assert.True(_sut.Validate(request).IsValid);
        }

        public static IEnumerable<object[]> GetInvalidRequests()
        {
            yield return new object[]
            {
                new RegisterRequest("", "Name Surname", "12345678910", "987Pjhmk?"),

                new List<string>
                {
                    "Username must have at least 3 characters"
                }
            };

            yield return new object[]
            {
                new RegisterRequest("Username", "Name", "12345678910", "987Pjhmk?"),

                new List<string>
                {
                    "Name must contain name and surname separated by space with at least 3 characters in each"
                }
            };

            yield return new object[]
            {
                new RegisterRequest("Username", "Name Surname", "1", "987Pjhmk?"),

                new List<string>
                {
                    "CPF number must contain exactly 11 digits"
                }
            };
            yield return new object[]
            {
                new RegisterRequest("Username", "Name Surname", "123456789a0", "987Pjhmk?"),

                new List<string>
                {
                    "CPF number must contain only digits"
                }
            };

            yield return new object[]
            {
                new RegisterRequest("Username", "Name Surname", "12345678910", "987Pjh?"),

                new List<string>
                {
                    "Password must be at least 8 characters long"
                }
            };

            yield return new object[]
            {
                new RegisterRequest("Username", "Name Surname", "12345678910", "987pjhmk?"),

                new List<string>
                {
                    "Password must contain at least one uppercase letter"
                }
            };

            yield return new object[]
            {
                new RegisterRequest("Username", "Name Surname", "12345678910", "987Pjhmk"),

                new List<string>
                {
                    "Password must contain at least one special character"
                }
            };

            yield return new object[]
            {
                new RegisterRequest("", "", "", ""),

                new List<string>
                {
                    "Username must have at least 3 characters",
                    "Name must contain name and surname separated by space with at least 3 characters in each",
                    "CPF number must contain exactly 11 digits",
                    "CPF number must contain only digits",
                    "Password must be at least 8 characters long",
                    "Password must contain at least one uppercase letter",
                    "Password must contain at least one special character"
                }
            };
        }
    }
}
