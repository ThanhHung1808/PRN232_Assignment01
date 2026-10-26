using FUNewsManagementClient.DTOs;
using FUNewsManagementClient.Helpers;
using FUNewsManagementClient.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FUNewsManagementClient.Pages.Staff.Categories
{
    public class IndexModel : PageModel
    {
        private readonly IApiService _apiService;

        public IndexModel(IApiService apiService)
        {
            _apiService = apiService;
        }

        public List<CategoryResponseDto> Categories { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? Keyword { get; set; }

        [BindProperty]
        public CategoryCreateUpdateDto CategoryInput { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var user = HttpContext.Session.GetObject<LoginResponse>("CurrentUser");
            if (user == null)
            {
                return RedirectToPage("/Login");
            }
            if (!string.Equals(user.Role, "Staff", StringComparison.OrdinalIgnoreCase))
            {
                TempData["ErrorMessage"] = "Access denied. Only Staff members can manage Categories.";
                return RedirectToPage("/Index");
            }

            var res = await _apiService.GetAsync<List<CategoryResponseDto>>("api/categories");
            if (res.IsSuccess && res.Data != null)
            {
                var query = res.Data.AsEnumerable();
                if (!string.IsNullOrWhiteSpace(Keyword))
                {
                    var kw = Keyword.Trim().ToLower();
                    query = query.Where(c => c.CategoryName.ToLower().Contains(kw) 
                                          || c.CategoryDesciption.ToLower().Contains(kw));
                }
                Categories = query.ToList();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            var user = HttpContext.Session.GetObject<LoginResponse>("CurrentUser");
            if (user == null || !string.Equals(user.Role, "Staff", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToPage("/Login");
            }

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Validation failed. Please enter all required fields.";
                return RedirectToPage(new { Keyword });
            }

            var res = await _apiService.PostAsync<object>("api/categories", CategoryInput);
            if (res.IsSuccess)
            {
                TempData["SuccessMessage"] = $"Category '{CategoryInput.CategoryName}' created successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = res.ErrorMessage ?? "Failed to create category.";
            }

            return RedirectToPage(new { Keyword });
        }

        public async Task<IActionResult> OnPostUpdateAsync()
        {
            var user = HttpContext.Session.GetObject<LoginResponse>("CurrentUser");
            if (user == null || !string.Equals(user.Role, "Staff", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToPage("/Login");
            }

            if (CategoryInput.CategoryId <= 0 || string.IsNullOrWhiteSpace(CategoryInput.CategoryName))
            {
                TempData["ErrorMessage"] = "Validation failed. Please enter all required fields.";
                return RedirectToPage(new { Keyword });
            }

            var res = await _apiService.PutAsync<object>($"api/categories/{CategoryInput.CategoryId}", CategoryInput);
            if (res.IsSuccess)
            {
                TempData["SuccessMessage"] = $"Category '{CategoryInput.CategoryName}' updated successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = res.ErrorMessage ?? "Failed to update category.";
            }

            return RedirectToPage(new { Keyword });
        }

        public async Task<IActionResult> OnPostDeleteAsync([FromForm] short categoryId)
        {
            var user = HttpContext.Session.GetObject<LoginResponse>("CurrentUser");
            if (user == null || !string.Equals(user.Role, "Staff", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToPage("/Login");
            }

            var res = await _apiService.DeleteAsync($"api/categories/{categoryId}");
            if (res.IsSuccess)
            {
                TempData["SuccessMessage"] = "Category deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = res.ErrorMessage ?? "Failed to delete category.";
            }

            return RedirectToPage(new { Keyword });
        }
    }
}
