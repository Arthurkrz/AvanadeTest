using FluentValidation;
using Identity.API.Core.Common;
using Identity.API.Core.Contracts.Repository;
using Identity.API.Core.Contracts.Service;
using Identity.API.Core.Entities;
using Identity.API.Core.Enum;

namespace Identity.API.Service
{
    public class BuyerService : IBuyerService
    {
        private readonly IBuyerRepository _buyerRepository;
        private readonly IPasswordHasher _passwordHasher;

        private readonly IValidator<UserRegisterRequest> _requestValidator;

        public BuyerService(IBuyerRepository buyerRepository, IPasswordHasher passwordHasher, IValidator<UserRegisterRequest> requestValidator)
        {
            _buyerRepository = buyerRepository;
            _passwordHasher = passwordHasher;
            _requestValidator = requestValidator;
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            var buyer = await _buyerRepository.GetByUsernameAsync(username);

            if (buyer is null) throw new IdentityApiException(ErrorMessages.BUYERNOTFOUND,
                                                              ErrorType.NotFound);

            if (buyer.LockoutEnd.HasValue &&
                buyer.LockoutEnd.Value > DateTime.UtcNow)
                throw new IdentityApiException(ErrorMessages.LOCKEDACCOUNT.Replace("{lockoutEnd}", buyer.LockoutEnd.ToString()),
                                               ErrorType.BusinessRuleViolation);

            if (!_passwordHasher.VerifyPassword(password, buyer.PasswordHash,
                                                          buyer.PasswordSalt,
                                                          buyer.HashParams))
            {
                buyer.FailedLoginCount++;

                if (buyer.FailedLoginCount >= 10)
                {
                    buyer.LockoutEnd = DateTime.UtcNow.AddDays(1).Date;
                    buyer.FailedLoginCount = 0;
                }

                await _buyerRepository.UpdateAsync(buyer);
                return false;
            }

            buyer.FailedLoginCount = 0;
            buyer.LockoutEnd = null;

            await _buyerRepository.UpdateAsync(buyer);
            return true;
        }

        public async Task<Buyer> RegisterAsync(UserRegisterRequest request)
        {
            var validationResult = _requestValidator.Validate(request);

            if (!validationResult.IsValid) throw new IdentityApiException(ErrorMessages.INVALIDREQUEST
                .Replace("{error}", string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))),
                ErrorType.BusinessRuleViolation);

            var (hash, salt, hashParams) = _passwordHasher.HashPassword(request.Password);

            var buyer = new Buyer(request.Username, request.Name, request.CPF, 
                                  request.Email, request.PhoneNumber, 
                                  request.DeliveryAddress, hash, 
                                  salt, "Argon2id", hashParams);

            await _buyerRepository.CreateAsync(buyer);
            return buyer;
        }

        public async Task<Buyer> GetByUsernameAsync(string username) =>
            await _buyerRepository.GetByUsernameAsync(username);

        public async Task<bool> IsExistingByCPFAsync(string cpf) =>
            await _buyerRepository.IsExistingByCPFAsync(cpf);
    }
}
