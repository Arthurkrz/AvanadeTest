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
        private readonly IBaseRepository<Buyer> _buyerRepository;
        private readonly IPasswordHasher _passwordHasher;

        private readonly IValidator<UserRegisterRequest> _requestValidator;

        public BuyerService(IBaseRepository<Buyer> buyerRepository, IPasswordHasher passwordHasher, IValidator<UserRegisterRequest> requestValidator)
        {
            _buyerRepository = buyerRepository;
            _passwordHasher = passwordHasher;
            _requestValidator = requestValidator;
        }

        public bool Login(string username, string password)
        {
            var buyer = _buyerRepository.GetByUsername(username);

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

                _buyerRepository.Update(buyer);
                return false;
            }

            buyer.FailedLoginCount = 0;
            buyer.LockoutEnd = null;

            _buyerRepository.Update(buyer);
            return true;
        }

        public Buyer Register(UserRegisterRequest request)
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

            _buyerRepository.Create(buyer);
            return buyer;
        }

        public Buyer GetByUsername(string username) =>
            _buyerRepository.GetByUsername(username);
    }
}
