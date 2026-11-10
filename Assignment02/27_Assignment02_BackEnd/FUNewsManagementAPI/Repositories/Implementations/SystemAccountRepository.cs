using FUNewsManagementAPI.DAO;
using FUNewsManagementAPI.Models;
using FUNewsManagementAPI.Repositories.Interfaces;

namespace FUNewsManagementAPI.Repositories.Implementations
{
    public class SystemAccountRepository : ISystemAccountRepository
    {
        private readonly FUNewsManagementDbContext _context;

        public SystemAccountRepository(FUNewsManagementDbContext context)
        {
            _context = context;
        }

        public Task<List<SystemAccount>> GetAccountsAsync() 
            => SystemAccountDAO.Instance.GetAccountsAsync(_context);

        public Task<SystemAccount?> GetAccountByIdAsync(short id) 
            => SystemAccountDAO.Instance.GetAccountByIdAsync(_context, id);

        public Task<SystemAccount?> GetAccountByEmailAsync(string email) 
            => SystemAccountDAO.Instance.GetAccountByEmailAsync(_context, email);

        public Task<SystemAccount?> LoginAsync(string email, string password) 
            => SystemAccountDAO.Instance.LoginAsync(_context, email, password);

        public Task<SystemAccount> AddAccountAsync(SystemAccount account) 
            => SystemAccountDAO.Instance.AddAccountAsync(_context, account);

        public Task<SystemAccount> UpdateAccountAsync(SystemAccount account) 
            => SystemAccountDAO.Instance.UpdateAccountAsync(_context, account);

        public Task<bool> DeleteAccountAsync(short id) 
            => SystemAccountDAO.Instance.DeleteAccountAsync(_context, id);

        public Task<bool> HasCreatedArticlesAsync(short accountId) 
            => SystemAccountDAO.Instance.HasCreatedArticlesAsync(_context, accountId);
    }
}
