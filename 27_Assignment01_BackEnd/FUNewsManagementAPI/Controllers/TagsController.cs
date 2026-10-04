using FUNewsManagementAPI.Models;
using FUNewsManagementAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace FUNewsManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagsController : ODataController
    {
        private readonly ITagRepository _tagRepository;

        public TagsController(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        [HttpGet]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<Tag>>> GetTags()
        {
            var tags = await _tagRepository.GetTagsAsync();
            return Ok(tags.AsQueryable());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Tag>> GetTag(int id)
        {
            var tag = await _tagRepository.GetTagByIdAsync(id);
            if (tag == null) return NotFound(new { message = $"Tag ID {id} not found." });
            return Ok(tag);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTag([FromBody] Tag tag)
        {
            var created = await _tagRepository.AddTagAsync(tag);
            return CreatedAtAction(nameof(GetTag), new { id = created.TagId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTag(int id, [FromBody] Tag tag)
        {
            tag.TagId = id;
            var updated = await _tagRepository.UpdateTagAsync(tag);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTag(int id)
        {
            var success = await _tagRepository.DeleteTagAsync(id);
            if (!success) return NotFound(new { message = $"Tag ID {id} not found." });
            return Ok(new { message = "Tag deleted successfully." });
        }
    }
}
