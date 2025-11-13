using FluentValidation;
using FluentValidation.Results;
using Identity.API.Core.Common;
using Identity.API.Core.Contracts.Repository;
using Identity.API.Core.Contracts.Service;
using Identity.API.Core.Entities;
using Identity.API.Core.Enum;
using Identity.API.Core.Validators;
using Identity.API.Service;
using Moq;

namespace Identity.API.Tests.Services
{
    public class BuyerServiceTests
    {
        private readonly Mock<IBaseRepository<Buyer>> _buyerRepositoryMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<IValidator<UserRegisterRequest>> _requestValidatorMock;
        private readonly IValidator<UserRegisterRequest> _requestValidator;
        private IBuyerService _sut;

        public BuyerServiceTests()
        {
            _buyerRepositoryMock = new Mock<IBaseRepository<Buyer>>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _requestValidatorMock = new Mock<IValidator<UserRegisterRequest>>();
            _requestValidator = new UserRegisterRequestValidator();

            _sut = new BuyerService(_buyerRepositoryMock.Object,
                                    _passwordHasherMock.Object,
                                    _requestValidatorMock.Object);
        }

        [Fact]
        public void Login_ShouldReturnTrue_WhenValidCredentials()
        {
            // Arrange
            var buyer = new Buyer
            (
                "Username", "Name", "CPF", "Email", 
                "PhoneNumber", "DeliveryAddress", 
                new byte[] { 1, 2, 3 }, 
                new byte[] { 1, 2, 3 }, 
                "HashAlgorithm", "HashParams"
            );

            _buyerRepositoryMock.Setup(x => x.GetByUsername(
                It.IsAny<string>())).Returns(buyer);

            _passwordHasherMock.Setup(x => x.VerifyPassword(
                It.IsAny<string>(), It.IsAny<byte[]>(),
                It.IsAny<byte[]>(), It.IsAny<string>())).Returns(true);

            // Act & Assert
            Assert.True(_sut.Login("Username", "Password"));
        }

        [Fact]
        public void Login_ShouldReturnFalse_WhenInvalidCredentials()
        {
            // Arrange
            var buyer = new Buyer
            (
                "Username", "Name", "CPF", "Email",
                "PhoneNumber", "DeliveryAddress",
                new byte[] { 1, 2, 3 },
                new byte[] { 1, 2, 3 },
                "HashAlgorithm", "HashParams"
            );

            _buyerRepositoryMock.Setup(x => x.GetByUsername(
                It.IsAny<string>())).Returns(buyer);

            _passwordHasherMock.Setup(x => x.VerifyPassword(
                It.IsAny<string>(), It.IsAny<byte[]>(),
                It.IsAny<byte[]>(), It.IsAny<string>())).Returns(false);

            // Act & Assert
            Assert.False(_sut.Login("Username", "Password"));
        }

        [Fact]
        public void Login_ShouldThrowException_WhenBuyerNotFound()
        {
            // Arrange
            _buyerRepositoryMock.Setup(x => x.GetByUsername(
                It.IsAny<string>())).Returns((Buyer)null!);

            // Act & Assert
            var ex = Assert.Throws<IdentityApiException>(() =>
                _sut.Login("NonExistentUser", "Password"));

            Assert.Equal(ErrorMessages.BUYERNOTFOUND, ex.Message);
            Assert.Equal(ErrorType.NotFound, ex.ErrorType);
        }

        [Fact]
        public void Login_ShouldThrowException_WhenAccountIsLocked()
        {
            var lockedBuyer = new Buyer
            (
                "Username", "Name", "CPF", "Email",
                "PhoneNumber", "DeliveryAddress",
                new byte[] { 1, 2, 3 },
                new byte[] { 1, 2, 3 },
                "HashAlgorithm", "HashParams"
            );

            lockedBuyer.LockoutEnd = DateTime.UtcNow.AddDays(1).Date;

            _buyerRepositoryMock.Setup(x => x.GetByUsername(
                It.IsAny<string>())).Returns(lockedBuyer);

            // Act & Assert
            var ex = Assert.Throws<IdentityApiException>(() =>
                _sut.Login("Username", "Password"));

            var expectedErrorMessage = ErrorMessages.LOCKEDACCOUNT
                .Replace("{lockoutEnd}", lockedBuyer.LockoutEnd.ToString());

            Assert.Equal(expectedErrorMessage, ex.Message);
            Assert.Equal(ErrorType.BusinessRuleViolation, ex.ErrorType);
        }

        [Fact]
        public void Login_ShouldLockAccount_WhenFailedLoginCountExceedsLimit()
        {
            // Arrange
            var lockedBuyer = new Buyer
            (
                "Username", "Name", "CPF", "Email",
                "PhoneNumber", "DeliveryAddress",
                new byte[] { 1, 2, 3 },
                new byte[] { 1, 2, 3 },
                "HashAlgorithm", "HashParams"
            );

            lockedBuyer.FailedLoginCount = 10;

            _buyerRepositoryMock.Setup(x => x.GetByUsername(
                It.IsAny<string>())).Returns(lockedBuyer);

            _passwordHasherMock.Setup(x => x.VerifyPassword(
                It.IsAny<string>(), It.IsAny<byte[]>(),
                It.IsAny<byte[]>(), It.IsAny<string>())).Returns(false);

            // Act & Assert
            Assert.False(_sut.Login("Username", "Password"));

            _buyerRepositoryMock.Verify(x => x.Update(
                It.IsAny<Buyer>()), Times.Once());
        }

        [Fact]
        public void Register_ShouldInvokeRepositoryAddMethod()
        {
            // Arrange
            _requestValidatorMock.Setup(v => v.Validate(
                It.IsAny<UserRegisterRequest>())).Returns(
                new ValidationResult());

            _passwordHasherMock.Setup(x => x.HashPassword(
                It.IsAny<string>())).Returns((
                    new byte[] { 1, 2, 3 },
                    new byte[] { 1, 2, 3 },
                    "HashParams"
                ));

            // Act
            _sut.Register(new UserRegisterRequest("Username", "Name", "CPF", "Password",
                                                  "Email", "PhoneNumber", "DeliveryAddress"));

            // Assert
            _buyerRepositoryMock.Verify(x => x.Create(
                It.IsAny<Buyer>()), Times.Once());
        }

        [Theory]
        [MemberData(nameof(GetInvalidRequests))]
        public void Register_ShouldThrowExceptionWithErrors_WhenValidationFails(UserRegisterRequest request, IList<string> expectedErrors)
        {
            // Arrange
            _sut = new BuyerService(_buyerRepositoryMock.Object,
                                    _passwordHasherMock.Object,
                                    _requestValidator);

            var expectedError = string.Join(", ", expectedErrors);

            // Act & Assert
            var ex = Assert.Throws<IdentityApiException>(() =>
                _sut.Register(request));

            var expectedMessage = ErrorMessages.INVALIDREQUEST
                .Replace("{error}", expectedError);

            Assert.Equal(expectedMessage, ex.Message);
            Assert.Equal(ErrorType.BusinessRuleViolation, ex.ErrorType);
        }

        public static IEnumerable<object[]> GetInvalidRequests()
        {
            yield return new object[]
            {
                new UserRegisterRequest("", "Name Surname", "12345678910", "987Pjhmk?", "Email@Email.com", "21998795324", "DeliveryAddress 123"),

                new List<string>
                {
                    "Username must have at least 3 characters"
                }
            };

            yield return new object[]
            {
                new UserRegisterRequest("Username", "Name", "12345678910", "987Pjhmk?", "Email@Email.com", "21998795324", "DeliveryAddress 123"),

                new List<string>
                {
                    "Name must contain name and surname separated by space with at least 3 characters in each"
                }
            };

            yield return new object[]
            {
                new UserRegisterRequest("Username", "Name Surname", "1", "987Pjhmk?", "Email@Email.com", "21998795324", "DeliveryAddress 123"),

                new List<string>
                {
                    "CPF number must contain exactly 11 digits"
                }
            };

            yield return new object[]
            {
                new UserRegisterRequest("Username", "Name Surname", "123456789a0", "987Pjhmk?", "Email@Email.com", "21998795324", "DeliveryAddress 123"),

                new List<string>
                {
                    "CPF number must contain only digits"
                }
            };

            yield return new object[]
            {
                new UserRegisterRequest("Username", "Name Surname", "12345678910", "987Pjh?", "Email@Email.com", "21998795324", "DeliveryAddress 123"),

                new List<string>
                {
                    "Password must be at least 8 characters long"
                }
            };

            yield return new object[]
            {
                new UserRegisterRequest("Username", "Name Surname", "12345678910", "987pjhmk?", "Email@Email.com", "21998795324", "DeliveryAddress 123"),

                new List<string>
                {
                    "Password must contain at least one uppercase letter"
                }
            };

            yield return new object[]
            {
                new UserRegisterRequest("Username", "Name Surname", "12345678910", "987Pjhmk", "Email@Email.com", "21998795324", "DeliveryAddress 123"),

                new List<string>
                {
                    "Password must contain at least one special character"
                }
            };

            yield return new object[]
            {
                new UserRegisterRequest("Username", "Name Surname", "12345678910", "987Pjhmk?", "InvalidEmail", "21998795324", "DeliveryAddress 123"),

                new List<string>
                {
                     "Email must be a valid email address"
                }
            };

            yield return new object[]
            {
                new UserRegisterRequest("Username", "Name Surname", "12345678910", "987Pjhmk?", "Email@Email.com", "21998795324a", "DeliveryAddress 123"),

                new List<string>
                {
                    "Phone Number must not contain letters"
                }
            };

            yield return new object[]
            {
                new UserRegisterRequest("Username", "Name Surname", "12345678910", "987Pjhmk?", "Email@Email.com", "2199879532", "DeliveryAddress 123"),

                new List<string>
                {
                    "Phone Number must contain at least 11 digits"
                }
            };

            yield return new object[]
            {
                new UserRegisterRequest("Username", "Name Surname", "12345678910", "987Pjhmk?", "Email@Email.com", "(21) 99879-53244", "DeliveryAddress 123"),

                new List<string>
                {
                    "Phone Number must not contain more than 15 digits"
                }
            };

            yield return new object[]
            {
                new UserRegisterRequest("Username", "Name Surname", "12345678910", "987Pjhmk?", "Email@Email.com", "21998795324", "DeliveryAddress"),

                new List<string>
                {
                    "Delivery Address must contain a street number"
                }
            };

            yield return new object[]
            {
                new UserRegisterRequest("Username", "Name Surname", "12345678910", "987Pjhmk?", "Email@Email.com", "21998795324", "123"),

                new List<string>
                {
                    "Delivery Address must be at least 5 characters long"
                }
            };

            yield return new object[]
            {
                new UserRegisterRequest("", "", "", "", "", "", ""),

                new List<string>
                {
                    "Username must have at least 3 characters",
                    "Name must contain name and surname separated by space with at least 3 characters in each",
                    "CPF number must contain exactly 11 digits",
                    "CPF number must contain only digits",
                    "Password must be at least 8 characters long",
                    "Password must contain at least one uppercase letter",
                    "Password must contain at least one special character",
                    "Email must be a valid email address",
                    "Phone Number must contain at least 11 digits",
                    "Delivery Address must contain a street number",
                    "Delivery Address must be at least 5 characters long"
                }
            };
        }
    }
}
