using FluentValidation;
using Stock.API.Core.Common;
using Stock.API.Core.Contracts.Repository;
using Stock.API.Core.Contracts.Service;
using Stock.API.Core.Entities;
using Stock.API.Core.Enum;

namespace Stock.API.Service
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IPasswordHasher _passwordHasher;

        private readonly IValidator<RegisterRequest> _requestValidator;

        public AdminService(IAdminRepository adminRepository, IPasswordHasher passwordHasher, IValidator<RegisterRequest> requestValidator)
        {
            _adminRepository = adminRepository;
            _passwordHasher = passwordHasher;
            _requestValidator = requestValidator;
        }

        public bool Login(string username, string password)
        {
            var admin = _adminRepository.GetByUsername(username);

            if (admin is null) throw new StockApiException(ErrorMessages.ADMINNOTFOUND, 
                                                           ErrorType.NotFound);

            if (admin.LockoutEnd.HasValue &&
                admin.LockoutEnd.Value > DateTime.UtcNow)
                throw new StockApiException(ErrorMessages.LOCKEDACCOUNT.Replace("{lockoutEnd}", admin.LockoutEnd.ToString()),
                                            ErrorType.BusinessRuleViolation);

            if (!_passwordHasher.VerifyPassword(password, admin.PasswordHash,
                                                          admin.PasswordSalt,
                                                          admin.HashParams))
            {
                admin.FailedLoginCount++;

                if (admin.FailedLoginCount >= 10)
                {
                    admin.LockoutEnd = DateTime.UtcNow.AddDays(1).Date;
                    admin.FailedLoginCount = 0;
                }

                _adminRepository.Update(admin);
                return false;
            }

            admin.FailedLoginCount = 0;
            admin.LockoutEnd = null;

            _adminRepository.Update(admin);
            return true;
        }

        public Admin Register(RegisterRequest request)
        {
            var validationResult = _requestValidator.Validate(request);

            if (!validationResult.IsValid) throw new StockApiException(ErrorMessages.INVALIDREQUEST
                .Replace("{error}", string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))), 
                ErrorType.BusinessRuleViolation);

            var (hash, salt, hashParams) = _passwordHasher.HashPassword(request.Password);

            var admin = new Admin(request.Username, request.Name, 
                                  request.CPF, hash, salt, 
                                  "Argon2id", hashParams);

            _adminRepository.Create(admin);
            return admin;
        }
    }
}
