using FUNewsManagementClient.DTOs;
using FUNewsManagementClient.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FUNewsManagementClient.Pages
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

        [BindProperty(SupportsGet = true)]
        public string? Keyword { get; set; }

        [BindProperty(SupportsGet = true)]
        public short? CategoryId { get; set; }

        public async Task OnGetAsync()
        {
            // Get active categories
            var catRes = await _apiService.GetAsync<List<CategoryResponseDto>>("api/categories?activeOnly=true");
            if (catRes.IsSuccess && catRes.Data != null)
            {
                Categories = catRes.Data;
            }

            // Get active articles with filter
            var query = "api/newsarticles?activeOnly=true";
            if (!string.IsNullOrWhiteSpace(Keyword))
            {
                query += $"&keyword={Uri.EscapeDataString(Keyword)}";
            }
            if (CategoryId.HasValue && CategoryId.Value > 0)
            {
                query += $"&categoryId={CategoryId.Value}";
            }

            var artRes = await _apiService.GetAsync<List<NewsArticleResponseDto>>(query);
            if (artRes.IsSuccess && artRes.Data != null)
            {
                Articles = artRes.Data;
            }
        }
    }
}
