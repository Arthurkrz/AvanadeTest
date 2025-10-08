using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Stock.API.Core.Common;
using Stock.API.Core.Contracts.Repository;
using Stock.API.Core.Contracts.Service;
using Stock.API.Core.Entities;
using Stock.API.Core.Enum;
using Stock.API.Service;

namespace Stock.API.Tests.Services
{
    public class AdminServiceTests
    {
        private readonly Mock<IAdminRepository> _adminRepositoryMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<IValidator<RegisterRequest>> _requestValidatorMock;
        private readonly IAdminService _sut;

        public AdminServiceTests()
        {
            _adminRepositoryMock = new Mock<IAdminRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _requestValidatorMock = new Mock<IValidator<RegisterRequest>>();

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton(_adminRepositoryMock.Object);
            serviceCollection.AddSingleton(_passwordHasherMock.Object);
            serviceCollection.AddSingleton(_requestValidatorMock.Object);

            _sut = new AdminService(_adminRepositoryMock.Object,
                                    _passwordHasherMock.Object,
                                    _requestValidatorMock.Object);
        }

        [Fact]
        public void Login_ShouldReturnTrue_WhenCredentialsAreValid()
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
            var ex = Assert.Throws<StockApiException>(() => 
                _sut.Login("Username", "Password"));

            Assert.Equal(ErrorMessages.INVALIDCREDENTIALS, ex.Message);
            Assert.Equal(ErrorType.BusinessRuleViolation, ex.ErrorType);
        }

        [Fact]
        public void Login_ShouldThrowException_WhenAdminNotFound()
        {
            // Arrange
            _adminRepositoryMock.Setup(x => x.GetByUsername(
                It.IsAny<string>())).Returns((Admin)null!);

            // Act & Assert
            var ex = Assert.Throws<StockApiException>(() =>
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
            var ex = Assert.Throws<StockApiException>(() => 
                _sut.Login("Username", "Password"));

            var expectedErrorMessage = ErrorMessages.LOCKEDACCOUNT
                .Replace("{lockoutEnd}", lockedAdmin.LockoutEnd.ToString());

            Assert.Equal(expectedErrorMessage, ex.Message);
            Assert.Equal(ErrorType.BusinessRuleViolation, ex.ErrorType);
        }

        [Fact]
        public void Login_ShouldAddFailedLoginCount_WhenPasswordIsInvalid()
        {
            // Arrange
            var admin = new Admin
            (
                "Username", "Name", "CPF",
                new byte[] { 1, 2, 3 },
                new byte[] { 1, 2, 3 },
                "HashAlgorithm", "Params"
            );

            _adminRepositoryMock.Setup(x => x.GetByUsername(
                It.IsAny<string>())).Returns(admin);

            _passwordHasherMock.Setup(x => x.VerifyPassword(
                It.IsAny<string>(), It.IsAny<byte[]>(),
                It.IsAny<byte[]>(), It.IsAny<string>())).Returns(false);

            // Act & Assert
            var ex = Assert.Throws<StockApiException>(() =>
                _sut.Login("Username", "Password"));

            _adminRepositoryMock.Verify(x => x.Update(
                It.IsAny<Admin>()), Times.Once());

            Assert.Equal(ErrorMessages.INVALIDCREDENTIALS, ex.Message);
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
            var ex = Assert.Throws<StockApiException>(() =>
                _sut.Login("Username", "Password"));

            _adminRepositoryMock.Verify(x => x.Update(
                It.IsAny<Admin>()), Times.Once());

            Assert.Equal(ErrorMessages.INVALIDCREDENTIALS, ex.Message);
            Assert.Equal(ErrorType.BusinessRuleViolation, ex.ErrorType);
        }

        [Fact]
        public void Register_ShouldInvokeRepositoryAddMethod()
        {
            // Arrange
            _requestValidatorMock.Setup(x => x.Validate(
                It.IsAny<RegisterRequest>())).Returns(
                    new ValidationResult());

            _passwordHasherMock.Setup(x => x.HashPassword(
                It.IsAny<string>())).Returns((
                    new byte[] { 1, 2, 3 },
                    new byte[] { 4, 5, 6 },
                    "HashParams"
                ));

            // Act
            _sut.Register(new RegisterRequest("Username", "Name", "CPF", "Password"));

            // Assert
            _adminRepositoryMock.Verify(x => x.Create(
                It.IsAny<Admin>()), Times.Once());
        }

        [Fact]
        public void Register_ShouldThrowException_WhenValidationFails()
        {
            // Arrange
            _requestValidatorMock.Setup(
                x => x.Validate(It.IsAny<RegisterRequest>())).Returns(
                new ValidationResult(new List<ValidationFailure>
                { new ValidationFailure("Username", "Username is required") }));

            // Act & Assert
            var ex = Assert.Throws<StockApiException>(() =>
                _sut.Register(new RegisterRequest("Username", "Name", "CPF", "Password")));

            var expectedMessage = ErrorMessages.INVALIDREQUEST
                .Replace("{error}", "Username is required");

            Assert.Equal(expectedMessage, ex.Message);
            Assert.Equal(ErrorType.BusinessRuleViolation, ex.ErrorType);
        }
    }
}
