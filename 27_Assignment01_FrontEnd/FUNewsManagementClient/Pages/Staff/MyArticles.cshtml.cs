using FUNewsManagementClient.DTOs;
using FUNewsManagementClient.Helpers;
using FUNewsManagementClient.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FUNewsManagementClient.Pages.Staff
{
    public class MyArticlesModel : PageModel
    {
        private readonly IApiService _apiService;

        public MyArticlesModel(IApiService apiService)
        {
            _apiService = apiService;
        }

        public List<NewsArticleResponseDto> MyArticles { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var user = HttpContext.Session.GetObject<LoginResponse>("CurrentUser");
            if (user == null || !string.Equals(user.Role, "Staff", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToPage("/Login");
            }

            var res = await _apiService.GetAsync<List<NewsArticleResponseDto>>($"api/newsarticles/author/{user.AccountId}");
            if (res.IsSuccess && res.Data != null)
            {
                MyArticles = res.Data;
            }

            return Page();
        }
    }
}
