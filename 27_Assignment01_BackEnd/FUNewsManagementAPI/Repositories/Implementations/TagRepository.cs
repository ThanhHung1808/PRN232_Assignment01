using FUNewsManagementAPI.DAO;
using FUNewsManagementAPI.Models;
using FUNewsManagementAPI.Repositories.Interfaces;

namespace FUNewsManagementAPI.Repositories.Implementations
{
    public class TagRepository : ITagRepository
    {
        public Task<List<Tag>> GetTagsAsync() => TagDAO.Instance.GetTagsAsync();

        public Task<Tag?> GetTagByIdAsync(int id) => TagDAO.Instance.GetTagByIdAsync(id);

        public Task<Tag> AddTagAsync(Tag tag) => TagDAO.Instance.AddTagAsync(tag);

        public Task<Tag> UpdateTagAsync(Tag tag) => TagDAO.Instance.UpdateTagAsync(tag);

        public Task<bool> DeleteTagAsync(int id) => TagDAO.Instance.DeleteTagAsync(id);
    }
}
