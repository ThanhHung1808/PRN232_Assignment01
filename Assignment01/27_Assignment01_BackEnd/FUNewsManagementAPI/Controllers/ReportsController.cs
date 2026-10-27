using FUNewsManagementAPI.DTOs;
using FUNewsManagementAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FUNewsManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly INewsArticleRepository _articleRepository;

        public ReportsController(INewsArticleRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }

        [HttpGet("statistics")]
        public async Task<ActionResult<ReportStatisticDto>> GetStatistics([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (startDate > endDate)
            {
                return BadRequest(new { message = "StartDate must be earlier than or equal to EndDate." });
            }

            var articles = await _articleRepository.GetReportAsync(startDate, endDate);
            var dtos = articles.Select(a => new NewsArticleResponseDto
            {
                NewsArticleId = a.NewsArticleId,
                NewsTitle = a.NewsTitle,
                Headline = a.Headline,
                CreatedDate = a.CreatedDate,
                NewsContent = a.NewsContent,
                NewsSource = a.NewsSource,
                CategoryId = a.CategoryId,
                CategoryName = a.Category?.CategoryName,
                NewsStatus = a.NewsStatus,
                CreatedById = a.CreatedById,
                CreatedByName = a.CreatedBy?.AccountName,
                UpdatedById = a.UpdatedById,
                ModifiedDate = a.ModifiedDate,
                Tags = a.NewsTags?.Select(nt => new TagDto
                {
                    TagId = nt.TagId,
                    TagName = nt.Tag?.TagName,
                    Note = nt.Tag?.Note
                }).ToList() ?? new List<TagDto>()
            }).ToList();

            var result = new ReportStatisticDto
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalArticles = dtos.Count,
                Articles = dtos
            };

            return Ok(result);
        }
    }
}
