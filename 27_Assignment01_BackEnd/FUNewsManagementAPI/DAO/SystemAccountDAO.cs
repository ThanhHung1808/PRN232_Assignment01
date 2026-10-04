using FUNewsManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagementAPI.DAO
{
    public class SystemAccountDAO
    {
        private static SystemAccountDAO? _instance;
        private static readonly object _lock = new object();
        private const string AdminEmail = "admin@FUNewsManagementSystem.org";

        private SystemAccountDAO() { }

        public static SystemAccountDAO Instance
        {
            get
            {
                lock (_lock)
                {
                    return _instance ??= new SystemAccountDAO();
                }
            }
        }

        public async Task<List<SystemAccount>> GetAccountsAsync()
        {
            using var context = new FUNewsManagementDbContext();
            return await context.SystemAccounts
                .Include(a => a.CreatedNewsArticles)
                .ToListAsync();
        }

        public async Task<SystemAccount?> GetAccountByIdAsync(short id)
        {
            using var context = new FUNewsManagementDbContext();
            return await context.SystemAccounts
                .Include(a => a.CreatedNewsArticles)
                .FirstOrDefaultAsync(a => a.AccountId == id);
        }

        public async Task<SystemAccount?> GetAccountByEmailAsync(string email)
        {
            using var context = new FUNewsManagementDbContext();
            return await context.SystemAccounts
                .FirstOrDefaultAsync(a => a.AccountEmail != null && a.AccountEmail.ToLower() == email.Trim().ToLower());
        }

        public async Task<SystemAccount?> LoginAsync(string email, string password)
        {
            using var context = new FUNewsManagementDbContext();
            return await context.SystemAccounts
                .FirstOrDefaultAsync(a => a.AccountEmail != null 
                    && a.AccountEmail.ToLower() == email.Trim().ToLower() 
                    && a.AccountPassword == password);
        }

        public async Task<SystemAccount> AddAccountAsync(SystemAccount account)
        {
            using var context = new FUNewsManagementDbContext();

            // 1. Check duplicate Account ID
            if (account.AccountId == 0)
            {
                short maxId = await context.SystemAccounts.AnyAsync() 
                    ? await context.SystemAccounts.MaxAsync(a => a.AccountId) 
                    : (short)0;
                account.AccountId = (short)(maxId + 1);
            }
            else
            {
                var existingId = await context.SystemAccounts.FindAsync(account.AccountId);
                if (existingId != null)
                {
                    throw new InvalidOperationException($"Account ID {account.AccountId} already exists.");
                }
            }

            // 2. Check collision with Admin email
            if (string.Equals(account.AccountEmail?.Trim(), AdminEmail, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"The email '{AdminEmail}' is reserved for the System Administrator.");
            }

            // 3. Check duplicate Email in database
            var existingEmail = await context.SystemAccounts
                .AnyAsync(a => a.AccountEmail != null && a.AccountEmail.Trim().ToLower() == account.AccountEmail!.Trim().ToLower());
            if (existingEmail)
            {
                throw new InvalidOperationException($"Email '{account.AccountEmail}' is already registered.");
            }

            context.SystemAccounts.Add(account);
            await context.SaveChangesAsync();
            return account;
        }

        public async Task<SystemAccount> UpdateAccountAsync(SystemAccount account)
        {
            using var context = new FUNewsManagementDbContext();
            var existing = await context.SystemAccounts.FindAsync(account.AccountId);
            if (existing == null)
            {
                throw new KeyNotFoundException($"Account with ID {account.AccountId} not found.");
            }

            if (!string.IsNullOrWhiteSpace(account.AccountEmail))
            {
                // Check collision with Admin email
                if (string.Equals(account.AccountEmail.Trim(), AdminEmail, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException($"The email '{AdminEmail}' is reserved for the System Administrator.");
                }

                // Check duplicate Email in database
                var emailTaken = await context.SystemAccounts
                    .AnyAsync(a => a.AccountId != account.AccountId && a.AccountEmail!.Trim().ToLower() == account.AccountEmail.Trim().ToLower());
                if (emailTaken)
                {
                    throw new InvalidOperationException($"Email '{account.AccountEmail}' is already taken by another account.");
                }
                existing.AccountEmail = account.AccountEmail.Trim();
            }

            if (!string.IsNullOrWhiteSpace(account.AccountName))
            {
                existing.AccountName = account.AccountName.Trim();
            }

            if (account.AccountRole.HasValue)
            {
                existing.AccountRole = account.AccountRole;
            }

            if (!string.IsNullOrWhiteSpace(account.AccountPassword))
            {
                existing.AccountPassword = account.AccountPassword;
            }

            await context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> HasCreatedArticlesAsync(short accountId)
        {
            using var context = new FUNewsManagementDbContext();
            return await context.NewsArticles.AnyAsync(n => n.CreatedById == accountId);
        }

        public async Task<bool> DeleteAccountAsync(short id)
        {
            using var context = new FUNewsManagementDbContext();
            var existing = await context.SystemAccounts.FindAsync(id);
            if (existing == null) return false;

            // Constraint: Cannot delete if account has created any articles
            var hasArticles = await context.NewsArticles.AnyAsync(n => n.CreatedById == id);
            if (hasArticles)
            {
                throw new InvalidOperationException("Cannot delete this account because it has created news articles.");
            }

            context.SystemAccounts.Remove(existing);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
