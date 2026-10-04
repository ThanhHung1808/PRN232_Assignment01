using FUNewsManagementClient.DTOs;
using FUNewsManagementClient.Models;
using FUNewsManagementClient.Helpers;
using FUNewsManagementClient.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FUNewsManagementClient.Pages.Staff.Articles
{
    public class IndexModel : PageModel
    {
        private readonly IApiService _apiService;

        public IndexModel(IApiService apiService)
        {
            _apiService = apiService;
        }

        public List<NewsArticleResponseDto> Articles { get; set; } = new();
        public List<CategoryResponseDto> Categories { get; set; } = new();
        public List<Tag> AvailableTags { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? Keyword { get; set; }

        [BindProperty(SupportsGet = true)]
        public short? CategoryId { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool? Status { get; set; }

        [BindProperty]
        public NewsArticleCreateUpdateDto ArticleInput { get; set; } = new();

        [BindProperty]
        public List<int> SelectedTagIds { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var user = HttpContext.Session.GetObject<LoginResponse>("CurrentUser");
            if (user == null || !string.Equals(user.Role, "Staff", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToPage("/Login");
            }

            // Load categories
            var catRes = await _apiService.GetAsync<List<CategoryResponseDto>>("api/categories");
            if (catRes.IsSuccess && catRes.Data != null) Categories = catRes.Data;

            // Load tags
            var tagRes = await _apiService.GetAsync<List<Tag>>("api/tags");
            if (tagRes.IsSuccess && tagRes.Data != null) AvailableTags = tagRes.Data;

            // Load articles
            var query = "api/newsarticles?";
            if (Status.HasValue) query += $"activeOnly={Status.Value}&";
            if (!string.IsNullOrWhiteSpace(Keyword)) query += $"keyword={Uri.EscapeDataString(Keyword)}&";
            if (CategoryId.HasValue && CategoryId.Value > 0) query += $"categoryId={CategoryId.Value}&";

            var artRes = await _apiService.GetAsync<List<NewsArticleResponseDto>>(query);
            if (artRes.IsSuccess && artRes.Data != null) Articles = artRes.Data;

            return Page();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            var user = HttpContext.Session.GetObject<LoginResponse>("CurrentUser");
            if (user == null || !string.Equals(user.Role, "Staff", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToPage("/Login");
            }

            ArticleInput.TagIds = SelectedTagIds;

            if (string.IsNullOrWhiteSpace(ArticleInput.NewsArticleId) || string.IsNullOrWhiteSpace(ArticleInput.Headline))
            {
                TempData["ErrorMessage"] = "Validation failed. Article ID and Headline are mandatory.";
                return RedirectToPage();
            }

            var res = await _apiService.PostAsync<object>($"api/newsarticles?createdById={user.AccountId}", ArticleInput);
            if (res.IsSuccess)
            {
                TempData["SuccessMessage"] = $"News Article '{ArticleInput.Headline}' created successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = res.ErrorMessage ?? "Failed to create news article.";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdateAsync()
        {
            var user = HttpContext.Session.GetObject<LoginResponse>("CurrentUser");
            if (user == null || !string.Equals(user.Role, "Staff", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToPage("/Login");
            }

            ArticleInput.TagIds = SelectedTagIds;

            if (string.IsNullOrWhiteSpace(ArticleInput.NewsArticleId) || string.IsNullOrWhiteSpace(ArticleInput.Headline))
            {
                TempData["ErrorMessage"] = "Validation failed. Article ID and Headline are mandatory.";
                return RedirectToPage();
            }

            var res = await _apiService.PutAsync<object>($"api/newsarticles/{ArticleInput.NewsArticleId}?updatedById={user.AccountId}", ArticleInput);
            if (res.IsSuccess)
            {
                TempData["SuccessMessage"] = $"News Article '{ArticleInput.Headline}' updated successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = res.ErrorMessage ?? "Failed to update news article.";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync([FromForm] string articleId)
        {
            var user = HttpContext.Session.GetObject<LoginResponse>("CurrentUser");
            if (user == null || !string.Equals(user.Role, "Staff", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToPage("/Login");
            }

            var res = await _apiService.DeleteAsync($"api/newsarticles/{articleId}");
            if (res.IsSuccess)
            {
                TempData["SuccessMessage"] = "News Article deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = res.ErrorMessage ?? "Failed to delete news article.";
            }

            return RedirectToPage();
        }
    }
}
