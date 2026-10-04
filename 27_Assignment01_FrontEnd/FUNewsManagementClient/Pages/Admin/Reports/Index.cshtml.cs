using FUNewsManagementClient.DTOs;
using FUNewsManagementClient.Helpers;
using FUNewsManagementClient.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FUNewsManagementClient.Pages.Admin.Reports
{
    public class IndexModel : PageModel
    {
        private readonly IApiService _apiService;

        public IndexModel(IApiService apiService)
        {
            _apiService = apiService;
        }

        [BindProperty(SupportsGet = true)]
        public DateTime StartDate { get; set; } = new DateTime(DateTime.Now.Year, 1, 1);

        [BindProperty(SupportsGet = true)]
        public DateTime EndDate { get; set; } = DateTime.Now;

        public ReportStatisticDto? ReportData { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = HttpContext.Session.GetObject<LoginResponse>("CurrentUser");
            if (user == null || !string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToPage("/Login");
            }

            var query = $"api/reports/statistics?startDate={StartDate:yyyy-MM-dd}&endDate={EndDate:yyyy-MM-dd}";
            var res = await _apiService.GetAsync<ReportStatisticDto>(query);
            if (res.IsSuccess && res.Data != null)
            {
                ReportData = res.Data;
            }

            return Page();
        }
    }
}
