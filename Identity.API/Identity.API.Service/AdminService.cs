using FluentValidation;
using Identity.API.Core.Common;
using Identity.API.Core.Contracts.Repository;
using Identity.API.Core.Contracts.Service;
using Identity.API.Core.Entities;
using Identity.API.Core.Enum;

namespace Identity.API.Service
{
    public class AdminService : IAdminService
    {
        private readonly IBaseRepository<Admin> _adminRepository;
        private readonly IPasswordHasher _passwordHasher;

        private readonly IValidator<AdminRegisterRequest> _requestValidator;

        public AdminService(IBaseRepository<Admin> adminRepository, IPasswordHasher passwordHasher, IValidator<AdminRegisterRequest> requestValidator)
        {
            _adminRepository = adminRepository;
            _passwordHasher = passwordHasher;
            _requestValidator = requestValidator;
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            var admin = await _adminRepository.GetByUsernameAsync(username);

            if (admin is null) throw new IdentityApiException(ErrorMessages.ADMINNOTFOUND,
                                                              ErrorType.NotFound);

            if (admin.LockoutEnd.HasValue &&
                admin.LockoutEnd.Value > DateTime.UtcNow)
                throw new IdentityApiException(ErrorMessages.LOCKEDACCOUNT.Replace("{lockoutEnd}", admin.LockoutEnd.ToString()),
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

                await _adminRepository.UpdateAsync(admin);
                return false;
            }

            admin.FailedLoginCount = 0;
            admin.LockoutEnd = null;

            await _adminRepository.UpdateAsync(admin);
            return true;
        }

        public async Task<Admin> RegisterAsync(AdminRegisterRequest request)
        {
            var validationResult = _requestValidator.Validate(request);

            if (!validationResult.IsValid) throw new IdentityApiException(ErrorMessages.INVALIDREQUEST
                .Replace("{error}", string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))),
                ErrorType.BusinessRuleViolation);

            var (hash, salt, hashParams) = _passwordHasher.HashPassword(request.Password);

            var admin = new Admin(request.Username, request.Name,
                                  request.CPF, hash, salt,
                                  "Argon2id", hashParams);

            await _adminRepository.CreateAsync(admin);
            return admin;
        }

        public async Task<Admin> GetByUsernameAsync(string username) =>
            await _adminRepository.GetByUsernameAsync(username);
    }
}
