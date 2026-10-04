using FUNewsManagementClient.DTOs;
using FUNewsManagementClient.Helpers;
using FUNewsManagementClient.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FUNewsManagementClient.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IApiService _apiService;

        public LoginModel(IApiService apiService)
        {
            _apiService = apiService;
        }

        [BindProperty]
        public LoginRequest LoginInput { get; set; } = new();

        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
            var user = HttpContext.Session.GetObject<LoginResponse>("CurrentUser");
            if (user != null)
            {
                if (string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase))
                {
                    Response.Redirect("/Admin/Accounts/Index");
                }
                else
                {
                    Response.Redirect("/Staff/Articles/Index");
                }
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var response = await _apiService.PostAsync<LoginResponse>("api/auth/login", LoginInput);
            if (!response.IsSuccess || response.Data == null)
            {
                ErrorMessage = response.ErrorMessage ?? "Invalid email or password.";
                return Page();
            }

            HttpContext.Session.SetObject("CurrentUser", response.Data);
            TempData["SuccessMessage"] = $"Welcome back, {response.Data.AccountName}!";

            if (string.Equals(response.Data.Role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToPage("/Admin/Accounts/Index");
            }

            return RedirectToPage("/Staff/Articles/Index");
        }
    }
}
