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
    public class CategoriesController : ODataController
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoriesController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<CategoryResponseDto>>> GetCategories([FromQuery] bool? activeOnly)
        {
            var categories = await _categoryRepository.GetCategoriesAsync(activeOnly);
            var dtos = categories.Select(c => new CategoryResponseDto
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                CategoryDesciption = c.CategoryDesciption,
                ParentCategoryId = c.ParentCategoryId,
                ParentCategoryName = c.ParentCategory?.CategoryName,
                IsActive = c.IsActive,
                ArticleCount = c.NewsArticles?.Count ?? 0
            });
            return Ok(dtos.AsQueryable());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryResponseDto>> GetCategory(short id)
        {
            var c = await _categoryRepository.GetCategoryByIdAsync(id);
            if (c == null)
            {
                return NotFound(new { message = $"Category with ID {id} not found." });
            }

            var dto = new CategoryResponseDto
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                CategoryDesciption = c.CategoryDesciption,
                ParentCategoryId = c.ParentCategoryId,
                ParentCategoryName = c.ParentCategory?.CategoryName,
                IsActive = c.IsActive,
                ArticleCount = c.NewsArticles?.Count ?? 0
            };
            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryCreateUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var category = new Category
                {
                    CategoryName = dto.CategoryName,
                    CategoryDesciption = dto.CategoryDesciption,
                    ParentCategoryId = dto.ParentCategoryId > 0 ? dto.ParentCategoryId : null,
                    IsActive = dto.IsActive ?? true
                };

                var created = await _categoryRepository.AddCategoryAsync(category);
                return CreatedAtAction(nameof(GetCategory), new { id = created.CategoryId }, created);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(short id, [FromBody] CategoryCreateUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var category = new Category
                {
                    CategoryId = id,
                    CategoryName = dto.CategoryName,
                    CategoryDesciption = dto.CategoryDesciption,
                    ParentCategoryId = dto.ParentCategoryId > 0 ? dto.ParentCategoryId : null,
                    IsActive = dto.IsActive
                };

                var updated = await _categoryRepository.UpdateCategoryAsync(category);
                return Ok(updated);
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
        public async Task<IActionResult> DeleteCategory(short id)
        {
            try
            {
                var hasArticles = await _categoryRepository.HasNewsArticlesAsync(id);
                if (hasArticles)
                {
                    return BadRequest(new { message = "Cannot delete this category because it contains news articles." });
                }

                var success = await _categoryRepository.DeleteCategoryAsync(id);
                if (!success)
                {
                    return NotFound(new { message = $"Category with ID {id} not found." });
                }

                return Ok(new { message = "Category deleted successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
