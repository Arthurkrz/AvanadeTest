using Stock.API.Core.Common;
using Stock.API.Core.Contracts.Repository;
using Stock.API.Core.Entities;
using Stock.API.Core.Enum;

namespace Stock.API.Architecture.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly Context _context;

        public AdminRepository(Context context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Admin Create(Admin admin)
        {
            if (!IsCPFUnique(admin.CPF) ||
                !IsNameUnique(admin.Name) ||
                !IsUsernameUnique(admin.Username))
                throw new StockApiException(ErrorMessages.ADMINNOTUNIQUE, 
                                            ErrorType.BusinessRuleViolation);

            _context.Admins.Add(admin);
            _context.SaveChanges();

            return admin;
        }

        public Admin GetByUsername(string username)
        {
            var entity = _context.Admins.Find(username);

            if (entity is null) 
                throw new StockApiException(ErrorMessages.ADMINNOTFOUND, 
                                            ErrorType.NotFound);
            return entity!;
        }

        private bool IsCPFUnique(string cpf) =>
            !_context.Admins.Any(e => e.CPF == cpf);

        private bool IsNameUnique(string name) =>
            !_context.Admins.Any(e => e.Name == name);

        private bool IsUsernameUnique(string username) =>
            !_context.Admins.Any(e => e.Username == username);
    }
}
