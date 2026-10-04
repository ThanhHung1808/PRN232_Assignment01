using FUNewsManagementAPI.Models;

namespace FUNewsManagementAPI.Repositories.Interfaces
{
    public interface ISystemAccountRepository
    {
        Task<List<SystemAccount>> GetAccountsAsync();
        Task<SystemAccount?> GetAccountByIdAsync(short id);
        Task<SystemAccount?> GetAccountByEmailAsync(string email);
        Task<SystemAccount?> LoginAsync(string email, string password);
        Task<SystemAccount> AddAccountAsync(SystemAccount account);
        Task<SystemAccount> UpdateAccountAsync(SystemAccount account);
        Task<bool> DeleteAccountAsync(short id);
        Task<bool> HasCreatedArticlesAsync(short accountId);
    }
}
