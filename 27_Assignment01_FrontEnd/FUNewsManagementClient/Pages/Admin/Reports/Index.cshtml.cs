using ClosedXML.Excel;
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

        public async Task<IActionResult> OnGetExportExcelAsync(DateTime startDate, DateTime endDate)
        {
            var user = HttpContext.Session.GetObject<LoginResponse>("CurrentUser");
            if (user == null || !string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToPage("/Login");
            }

            var query = $"api/reports/statistics?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";
            var res = await _apiService.GetAsync<ReportStatisticDto>(query);
            if (!res.IsSuccess || res.Data == null || !res.Data.Articles.Any())
            {
                TempData["ErrorMessage"] = "No article data found in this period to export.";
                return RedirectToPage(new { StartDate = startDate.ToString("yyyy-MM-dd"), EndDate = endDate.ToString("yyyy-MM-dd") });
            }

            var report = res.Data;

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Article Statistics");

            // 1. Report Title
            ws.Cell(1, 1).Value = "FU NEWS MANAGEMENT SYSTEM - ARTICLE REPORT";
            ws.Cell(1, 1).Style.Font.Bold = true;
            ws.Cell(1, 1).Style.Font.FontSize = 16;
            ws.Cell(1, 1).Style.Font.FontColor = XLColor.FromArgb(13, 110, 253);

            // 2. Metadata Info
            ws.Cell(2, 1).Value = $"Report Period: {report.StartDate:dd/MM/yyyy} - {report.EndDate:dd/MM/yyyy}";
            ws.Cell(2, 1).Style.Font.Italic = true;

            ws.Cell(3, 1).Value = $"Total Articles: {report.TotalArticles} | Exported At: {DateTime.Now:dd/MM/yyyy HH:mm}";
            ws.Cell(3, 1).Style.Font.Italic = true;

            // 3. Table Headers
            int headerRow = 5;
            string[] headers = { "#", "Article ID", "Created Date", "Headline", "Full Title", "Category", "Author (Created By)", "Status", "Tags", "Source" };
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cell(headerRow, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Fill.BackgroundColor = XLColor.FromArgb(13, 110, 253);
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }

            // 4. Data Rows
            int currentRow = headerRow + 1;
            int index = 1;
            foreach (var art in report.Articles)
            {
                ws.Cell(currentRow, 1).Value = index++;
                ws.Cell(currentRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell(currentRow, 2).Value = art.NewsArticleId;
                ws.Cell(currentRow, 3).Value = art.CreatedDate.HasValue ? art.CreatedDate.Value.ToString("dd/MM/yyyy HH:mm") : "N/A";
                ws.Cell(currentRow, 4).Value = art.Headline ?? "";
                ws.Cell(currentRow, 5).Value = art.NewsTitle ?? "";
                ws.Cell(currentRow, 6).Value = art.CategoryName ?? "N/A";
                ws.Cell(currentRow, 7).Value = art.CreatedByName ?? "N/A";
                
                var statusCell = ws.Cell(currentRow, 8);
                statusCell.Value = art.NewsStatus == true ? "Active" : "Inactive";
                statusCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                statusCell.Style.Font.Bold = true;
                if (art.NewsStatus == true)
                {
                    statusCell.Style.Font.FontColor = XLColor.FromArgb(25, 135, 84);
                }
                else
                {
                    statusCell.Style.Font.FontColor = XLColor.FromArgb(220, 53, 69);
                }

                ws.Cell(currentRow, 9).Value = art.Tags != null && art.Tags.Any() ? string.Join(", ", art.Tags.Select(t => "#" + t.TagName)) : "";
                ws.Cell(currentRow, 10).Value = art.NewsSource ?? "";

                // Cell borders
                for (int c = 1; c <= headers.Length; c++)
                {
                    ws.Cell(currentRow, c).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    ws.Cell(currentRow, c).Style.Border.OutsideBorderColor = XLColor.LightGray;
                }

                currentRow++;
            }

            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            string fileName = $"NewsArticles_Report_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.xlsx";
            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}
