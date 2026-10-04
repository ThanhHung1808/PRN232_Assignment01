using FUNewsManagementAPI.DTOs;
using FUNewsManagementAPI.Models;
using FUNewsManagementAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace FUNewsManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NewsArticlesController : ODataController
    {
        private readonly INewsArticleRepository _articleRepository;

        public NewsArticlesController(INewsArticleRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }

        [HttpGet]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<NewsArticleResponseDto>>> GetArticles(
            [FromQuery] bool? activeOnly,
            [FromQuery] string? keyword,
            [FromQuery] short? categoryId,
            [FromQuery] int? tagId)
        {
            var articles = await _articleRepository.GetArticlesAsync(activeOnly, keyword, categoryId, tagId);
            var dtos = articles.Select(MapToResponseDto);
            return Ok(dtos.AsQueryable());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NewsArticleResponseDto>> GetArticle(string id)
        {
            var article = await _articleRepository.GetArticleByIdAsync(id);
            if (article == null)
            {
                return NotFound(new { message = $"News Article with ID {id} not found." });
            }
            return Ok(MapToResponseDto(article));
        }

        [HttpGet("author/{authorId}")]
        public async Task<ActionResult<IEnumerable<NewsArticleResponseDto>>> GetArticlesByAuthor(short authorId)
        {
            var articles = await _articleRepository.GetArticlesByAuthorAsync(authorId);
            var dtos = articles.Select(MapToResponseDto);
            return Ok(dtos);
        }

        [HttpPost]
        public async Task<IActionResult> CreateArticle([FromBody] NewsArticleCreateUpdateDto dto, [FromQuery] short? createdById)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var article = new NewsArticle
                {
                    NewsArticleId = dto.NewsArticleId,
                    NewsTitle = dto.NewsTitle,
                    Headline = dto.Headline,
                    NewsContent = dto.NewsContent,
                    NewsSource = dto.NewsSource,
                    CategoryId = dto.CategoryId,
                    NewsStatus = dto.NewsStatus ?? true,
                    CreatedById = createdById,
                    UpdatedById = createdById,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now
                };

                var created = await _articleRepository.AddArticleAsync(article, dto.TagIds);
                return CreatedAtAction(nameof(GetArticle), new { id = created.NewsArticleId }, MapToResponseDto(created));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateArticle(string id, [FromBody] NewsArticleCreateUpdateDto dto, [FromQuery] short? updatedById)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var article = new NewsArticle
                {
                    NewsArticleId = id,
                    NewsTitle = dto.NewsTitle,
                    Headline = dto.Headline,
                    NewsContent = dto.NewsContent,
                    NewsSource = dto.NewsSource,
                    CategoryId = dto.CategoryId,
                    NewsStatus = dto.NewsStatus,
                    UpdatedById = updatedById,
                    ModifiedDate = DateTime.Now
                };

                var updated = await _articleRepository.UpdateArticleAsync(article, dto.TagIds);
                return Ok(MapToResponseDto(updated));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle(string id)
        {
            try
            {
                var success = await _articleRepository.DeleteArticleAsync(id);
                if (!success)
                {
                    return NotFound(new { message = $"News Article with ID {id} not found." });
                }

                return Ok(new { message = "News Article deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        private static NewsArticleResponseDto MapToResponseDto(NewsArticle a)
        {
            return new NewsArticleResponseDto
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
            };
        }
    }
}
