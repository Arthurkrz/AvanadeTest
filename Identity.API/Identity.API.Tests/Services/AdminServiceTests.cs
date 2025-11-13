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
    public class AdminServiceTests
    {
        private readonly Mock<IBaseRepository<Admin>> _adminRepositoryMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<IValidator<AdminRegisterRequest>> _requestValidatorMock;
        private readonly IValidator<AdminRegisterRequest> _requestValidator;
        private IAdminService _sut;

        public AdminServiceTests()
        {
            _adminRepositoryMock = new Mock<IBaseRepository<Admin>>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _requestValidatorMock = new Mock<IValidator<AdminRegisterRequest>>();
            _requestValidator = new AdminRegisterRequestValidator();

            _sut = new AdminService(_adminRepositoryMock.Object,
                                    _passwordHasherMock.Object,
                                    _requestValidatorMock.Object);
        }

        [Fact]
        public void Login_ShouldReturnTrue_WhenValidCredentials()
        {
            // Arrange
            var admin = new Admin
            (
                "Username", "Name", "CPF",
                new byte[] { 1, 2, 3 },
                new byte[] { 1, 2, 3 },
                "HashAlgorithm", "HashParams"
            );

            _adminRepositoryMock.Setup(x => x.GetByUsername(
                It.IsAny<string>())).Returns(admin);

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
            var admin = new Admin
            (
                "Username", "Name", "CPF",
                new byte[] { 1, 2, 3 },
                new byte[] { 1, 2, 3 },
                "HashAlgorithm", "HashParams"
            );

            _adminRepositoryMock.Setup(x => x.GetByUsername(
                It.IsAny<string>())).Returns(admin);

            _passwordHasherMock.Setup(x => x.VerifyPassword(
                It.IsAny<string>(), It.IsAny<byte[]>(),
                It.IsAny<byte[]>(), It.IsAny<string>())).Returns(false);

            // Act & Assert
            Assert.False(_sut.Login("Username", "Password"));
        }

        [Fact]
        public void Login_ShouldThrowException_WhenAdminNotFound()
        {
            // Arrange
            _adminRepositoryMock.Setup(x => x.GetByUsername(
                It.IsAny<string>())).Returns((Admin)null!);

            // Act & Assert
            var ex = Assert.Throws<IdentityApiException>(() =>
                _sut.Login("Username", "Password"));

            Assert.Equal(ErrorMessages.ADMINNOTFOUND, ex.Message);
            Assert.Equal(ErrorType.NotFound, ex.ErrorType);
        }

        [Fact]
        public void Login_ShouldThrowException_WhenAccountIsLocked()
        {
            // Arrange
            var lockedAdmin = new Admin
            (
                "Username", "Name", "CPF",
                new byte[] { 1, 2, 3 },
                new byte[] { 1, 2, 3 },
                "HashAlgorithm", "Params"
            );

            lockedAdmin.LockoutEnd = DateTime.UtcNow.AddDays(1).Date;

            _adminRepositoryMock.Setup(x => x.GetByUsername(
                It.IsAny<string>())).Returns(lockedAdmin);

            // Act & Assert
            var ex = Assert.Throws<IdentityApiException>(() =>
                _sut.Login("Username", "Password"));

            var expectedErrorMessage = ErrorMessages.LOCKEDACCOUNT
                .Replace("{lockoutEnd}", lockedAdmin.LockoutEnd.ToString());

            Assert.Equal(expectedErrorMessage, ex.Message);
            Assert.Equal(ErrorType.BusinessRuleViolation, ex.ErrorType);
        }

        [Fact]
        public void Login_ShouldLockAccount_WhenFailedLoginCountExceedsLimit()
        {
            // Arrange
            var lockedAdmin = new Admin
            (
                "Username", "Name", "CPF",
                new byte[] { 1, 2, 3 },
                new byte[] { 1, 2, 3 },
                "HashAlgorithm", "Params"
            );

            lockedAdmin.FailedLoginCount = 10;

            _adminRepositoryMock.Setup(x => x.GetByUsername(
                It.IsAny<string>())).Returns(lockedAdmin);

            _passwordHasherMock.Setup(x => x.VerifyPassword(
                It.IsAny<string>(), It.IsAny<byte[]>(),
                It.IsAny<byte[]>(), It.IsAny<string>())).Returns(false);

            // Act & Assert
            Assert.False(_sut.Login("Username", "Password"));

            _adminRepositoryMock.Verify(x => x.Update(
                It.IsAny<Admin>()), Times.Once());
        }

        [Fact]
        public void Register_ShouldInvokeRepositoryAddMethod()
        {
            // Arrange
            _requestValidatorMock.Setup(x => x.Validate(
                It.IsAny<AdminRegisterRequest>())).Returns(
                    new ValidationResult());

            _passwordHasherMock.Setup(x => x.HashPassword(
                It.IsAny<string>())).Returns((
                    new byte[] { 1, 2, 3 },
                    new byte[] { 4, 5, 6 },
                    "HashParams"
                ));

            // Act
            _sut.Register(new AdminRegisterRequest("Username", "Name", "CPF", "Password"));

            // Assert
            _adminRepositoryMock.Verify(x => x.Create(
                It.IsAny<Admin>()), Times.Once());
        }

        [Theory]
        [MemberData(nameof(GetInvalidRequests))]
        public void Register_ShouldThrowExceptionWithErrors_WhenValidationFails(AdminRegisterRequest request, IList<string> expectedErrors)
        {
            // Arrange
            _sut = new AdminService(_adminRepositoryMock.Object,
                                    _passwordHasherMock.Object,
                                    _requestValidator);

            var expectedError = string.Join(", ", expectedErrors);

            // Act & Assert
            var ex = Assert.Throws<IdentityApiException>(() =>
                _sut.Register(request));

            var expectedMessage = ErrorMessages.INVALIDREQUEST
                .Replace("{error}", expectedError);

            Assert.Equal(expectedMessage, ex.Error);
            Assert.Equal(ErrorType.BusinessRuleViolation, ex.ErrorType);
        }

        public static IEnumerable<object[]> GetInvalidRequests()
        {
            yield return new object[]
            {
                new AdminRegisterRequest("", "Name Surname", "12345678910", "987Pjhmk?"),

                new List<string>
                {
                    "Username must have at least 3 characters"
                }
            };

            yield return new object[]
            {
                new AdminRegisterRequest("Username", "Name", "12345678910", "987Pjhmk?"),

                new List<string>
                {
                    "Name must contain name and surname separated by space with at least 3 characters in each"
                }
            };

            yield return new object[]
            {
                new AdminRegisterRequest("Username", "Name Surname", "1", "987Pjhmk?"),

                new List<string>
                {
                    "CPF number must contain exactly 11 digits"
                }
            };
            yield return new object[]
            {
                new AdminRegisterRequest("Username", "Name Surname", "123456789a0", "987Pjhmk?"),

                new List<string>
                {
                    "CPF number must contain only digits"
                }
            };

            yield return new object[]
            {
                new AdminRegisterRequest("Username", "Name Surname", "12345678910", "987Pjh?"),

                new List<string>
                {
                    "Password must be at least 8 characters long"
                }
            };

            yield return new object[]
            {
                new AdminRegisterRequest("Username", "Name Surname", "12345678910", "987pjhmk?"),

                new List<string>
                {
                    "Password must contain at least one uppercase letter"
                }
            };

            yield return new object[]
            {
                new AdminRegisterRequest("Username", "Name Surname", "12345678910", "987Pjhmk"),

                new List<string>
                {
                    "Password must contain at least one special character"
                }
            };

            yield return new object[]
            {
                new AdminRegisterRequest("", "", "", ""),

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