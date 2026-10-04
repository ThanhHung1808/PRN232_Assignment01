using FUNewsManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagementAPI.DAO
{
    public class TagDAO
    {
        private static TagDAO? _instance;
        private static readonly object _lock = new object();

        private TagDAO() { }

        public static TagDAO Instance
        {
            get
            {
                lock (_lock)
                {
                    return _instance ??= new TagDAO();
                }
            }
        }

        public async Task<List<Tag>> GetTagsAsync()
        {
            using var context = new FUNewsManagementDbContext();
            return await context.Tags.ToListAsync();
        }

        public async Task<Tag?> GetTagByIdAsync(int id)
        {
            using var context = new FUNewsManagementDbContext();
            return await context.Tags.FindAsync(id);
        }

        public async Task<Tag> AddTagAsync(Tag tag)
        {
            using var context = new FUNewsManagementDbContext();

            if (!string.IsNullOrWhiteSpace(tag.TagName))
            {
                var tagExists = await context.Tags.AnyAsync(t => t.TagName != null && t.TagName.Trim().ToLower() == tag.TagName.Trim().ToLower());
                if (tagExists)
                {
                    throw new InvalidOperationException($"Tag name '{tag.TagName}' already exists.");
                }
            }

            if (tag.TagId == 0)
            {
                int maxId = await context.Tags.AnyAsync() ? await context.Tags.MaxAsync(t => t.TagId) : 0;
                tag.TagId = maxId + 1;
            }
            else
            {
                var existingId = await context.Tags.FindAsync(tag.TagId);
                if (existingId != null)
                {
                    throw new InvalidOperationException($"Tag ID {tag.TagId} already exists.");
                }
            }

            context.Tags.Add(tag);
            await context.SaveChangesAsync();
            return tag;
        }

        public async Task<Tag> UpdateTagAsync(Tag tag)
        {
            using var context = new FUNewsManagementDbContext();
            var existing = await context.Tags.FindAsync(tag.TagId);
            if (existing == null) throw new KeyNotFoundException($"Tag ID {tag.TagId} not found.");

            if (!string.IsNullOrWhiteSpace(tag.TagName))
            {
                var tagExists = await context.Tags.AnyAsync(t => t.TagId != tag.TagId && t.TagName != null && t.TagName.Trim().ToLower() == tag.TagName.Trim().ToLower());
                if (tagExists)
                {
                    throw new InvalidOperationException($"Tag name '{tag.TagName}' is already taken.");
                }
            }

            existing.TagName = tag.TagName;
            existing.Note = tag.Note;
            await context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteTagAsync(int id)
        {
            using var context = new FUNewsManagementDbContext();
            var existing = await context.Tags.FindAsync(id);
            if (existing == null) return false;

            var related = context.NewsTags.Where(nt => nt.TagId == id);
            context.NewsTags.RemoveRange(related);

            context.Tags.Remove(existing);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
