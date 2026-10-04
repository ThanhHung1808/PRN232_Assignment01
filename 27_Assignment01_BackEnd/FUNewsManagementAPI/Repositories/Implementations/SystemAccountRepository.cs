using FUNewsManagementAPI.DAO;
using FUNewsManagementAPI.Models;
using FUNewsManagementAPI.Repositories.Interfaces;

namespace FUNewsManagementAPI.Repositories.Implementations
{
    public class SystemAccountRepository : ISystemAccountRepository
    {
        public Task<List<SystemAccount>> GetAccountsAsync() 
            => SystemAccountDAO.Instance.GetAccountsAsync();

        public Task<SystemAccount?> GetAccountByIdAsync(short id) 
            => SystemAccountDAO.Instance.GetAccountByIdAsync(id);

        public Task<SystemAccount?> GetAccountByEmailAsync(string email) 
            => SystemAccountDAO.Instance.GetAccountByEmailAsync(email);

        public Task<SystemAccount?> LoginAsync(string email, string password) 
            => SystemAccountDAO.Instance.LoginAsync(email, password);

        public Task<SystemAccount> AddAccountAsync(SystemAccount account) 
            => SystemAccountDAO.Instance.AddAccountAsync(account);

        public Task<SystemAccount> UpdateAccountAsync(SystemAccount account) 
            => SystemAccountDAO.Instance.UpdateAccountAsync(account);

        public Task<bool> DeleteAccountAsync(short id) 
            => SystemAccountDAO.Instance.DeleteAccountAsync(id);

        public Task<bool> HasCreatedArticlesAsync(short accountId) 
            => SystemAccountDAO.Instance.HasCreatedArticlesAsync(accountId);
    }
}
