using FUNewsManagementAPI.Models;

namespace FUNewsManagementAPI.Repositories.Interfaces
{
    public interface ITagRepository
    {
        Task<List<Tag>> GetTagsAsync();
        Task<Tag?> GetTagByIdAsync(int id);
        Task<Tag> AddTagAsync(Tag tag);
        Task<Tag> UpdateTagAsync(Tag tag);
        Task<bool> DeleteTagAsync(int id);
    }
}
