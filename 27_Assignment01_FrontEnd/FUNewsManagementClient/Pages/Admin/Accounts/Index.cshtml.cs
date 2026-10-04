using FUNewsManagementClient.DTOs;
using FUNewsManagementClient.Helpers;
using FUNewsManagementClient.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FUNewsManagementClient.Pages.Admin.Accounts
{
    public class IndexModel : PageModel
    {
        private readonly IApiService _apiService;

        public IndexModel(IApiService apiService)
        {
            _apiService = apiService;
        }

        public List<AccountResponseDto> Accounts { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? Keyword { get; set; }

        [BindProperty]
        public AccountCreateUpdateDto AccountInput { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var user = HttpContext.Session.GetObject<LoginResponse>("CurrentUser");
            if (user == null || !string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToPage("/Login");
            }

            var res = await _apiService.GetAsync<List<AccountResponseDto>>("api/accounts");
            if (res.IsSuccess && res.Data != null)
            {
                var query = res.Data.AsEnumerable();
                if (!string.IsNullOrWhiteSpace(Keyword))
                {
                    var kw = Keyword.Trim().ToLower();
                    query = query.Where(a => (a.AccountName != null && a.AccountName.ToLower().Contains(kw))
                                          || (a.AccountEmail != null && a.AccountEmail.ToLower().Contains(kw)));
                }
                Accounts = query.ToList();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            var user = HttpContext.Session.GetObject<LoginResponse>("CurrentUser");
            if (user == null || !string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToPage("/Login");
            }

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Validation failed. Please check your input fields.";
                return RedirectToPage(new { Keyword });
            }

            var res = await _apiService.PostAsync<object>("api/accounts", AccountInput);
            if (res.IsSuccess)
            {
                TempData["SuccessMessage"] = $"Account '{AccountInput.AccountName}' created successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = res.ErrorMessage ?? "Failed to create account.";
            }

            return RedirectToPage(new { Keyword });
        }

        public async Task<IActionResult> OnPostUpdateAsync()
        {
            var user = HttpContext.Session.GetObject<LoginResponse>("CurrentUser");
            if (user == null || !string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToPage("/Login");
            }

            if (AccountInput.AccountId <= 0 || string.IsNullOrWhiteSpace(AccountInput.AccountName) || string.IsNullOrWhiteSpace(AccountInput.AccountEmail))
            {
                TempData["ErrorMessage"] = "Validation failed. Please fill all required fields.";
                return RedirectToPage(new { Keyword });
            }

            var res = await _apiService.PutAsync<object>($"api/accounts/{AccountInput.AccountId}", AccountInput);
            if (res.IsSuccess)
            {
                TempData["SuccessMessage"] = $"Account '{AccountInput.AccountName}' updated successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = res.ErrorMessage ?? "Failed to update account.";
            }

            return RedirectToPage(new { Keyword });
        }

        public async Task<IActionResult> OnPostDeleteAsync([FromForm] short accountId)
        {
            var user = HttpContext.Session.GetObject<LoginResponse>("CurrentUser");
            if (user == null || !string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToPage("/Login");
            }

            var res = await _apiService.DeleteAsync($"api/accounts/{accountId}");
            if (res.IsSuccess)
            {
                TempData["SuccessMessage"] = "Account deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = res.ErrorMessage ?? "Failed to delete account.";
            }

            return RedirectToPage(new { Keyword });
        }
    }
}
