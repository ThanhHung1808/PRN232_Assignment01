using System.ComponentModel.DataAnnotations;

namespace FUNewsManagementAPI.DTOs
{
    public class ReportFilterDto
    {
        [Required(ErrorMessage = "Start Date is required")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End Date is required")]
        public DateTime EndDate { get; set; }
    }

    public class ReportStatisticDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalArticles { get; set; }
        public List<NewsArticleResponseDto> Articles { get; set; } = new List<NewsArticleResponseDto>();
    }
}
