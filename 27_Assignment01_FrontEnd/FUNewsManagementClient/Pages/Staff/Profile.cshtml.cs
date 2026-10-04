using FUNewsManagementClient.DTOs;
using FUNewsManagementClient.Models;
using FUNewsManagementClient.Helpers;
using FUNewsManagementClient.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FUNewsManagementClient.Pages.Staff
{
    public class ProfileModel : PageModel
    {
        private readonly IApiService _apiService;

        public ProfileModel(IApiService apiService)
        {
            _apiService = apiService;
        }

        [BindProperty]
        public ProfileUpdateDto ProfileInput { get; set; } = new();

        public SystemAccount? AccountDetails { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = HttpContext.Session.GetObject<LoginResponse>("CurrentUser");
            if (user == null || !string.Equals(user.Role, "Staff", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToPage("/Login");
            }

            var res = await _apiService.GetAsync<SystemAccount>($"api/accounts/{user.AccountId}");
            if (res.IsSuccess && res.Data != null)
            {
                AccountDetails = res.Data;
                ProfileInput.AccountName = res.Data.AccountName ?? "";
                ProfileInput.AccountEmail = res.Data.AccountEmail ?? "";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = HttpContext.Session.GetObject<LoginResponse>("CurrentUser");
            if (user == null || !string.Equals(user.Role, "Staff", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToPage("/Login");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var res = await _apiService.PutAsync<object>($"api/accounts/profile/{user.AccountId}", ProfileInput);
            if (res.IsSuccess)
            {
                user.AccountName = ProfileInput.AccountName;
                user.AccountEmail = ProfileInput.AccountEmail;
                HttpContext.Session.SetObject("CurrentUser", user);
                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToPage();
            }

            TempData["ErrorMessage"] = res.ErrorMessage ?? "Failed to update profile.";
            return Page();
        }
    }
}
