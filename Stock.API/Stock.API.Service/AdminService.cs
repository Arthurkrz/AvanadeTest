using FluentValidation;
using Stock.API.Core.Common;
using Stock.API.Core.Contracts.Repository;
using Stock.API.Core.Contracts.Service;
using Stock.API.Core.Entities;

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

            if (admin is null) throw new Exception("");

            return _passwordHasher.VerifyPassword(password, admin.PasswordHash,
                                                  admin.PasswordSalt,
                                                  admin.HashParams);
        }

        public Admin Register(RegisterRequest request)
        {
            var validationResult = _requestValidator.Validate(request);

            if (!validationResult.IsValid) throw new Exception("");

            var (hash, salt, hashParams) = _passwordHasher.HashPassword(request.Password);

            var admin = new Admin(request.Username, request.Name, 
                                  request.CPF, hash, salt, 
                                  "Argon2id", hashParams);

            _adminRepository.Create(admin);
            return admin;
        }
    }
}
