using FUNewsManagementAPI.DAO;
using FUNewsManagementAPI.Models;
using FUNewsManagementAPI.Repositories.Interfaces;

namespace FUNewsManagementAPI.Repositories.Implementations
{
    public class TagRepository : ITagRepository
    {
        private readonly FUNewsManagementDbContext _context;

        public TagRepository(FUNewsManagementDbContext context)
        {
            _context = context;
        }

        public Task<List<Tag>> GetTagsAsync() => TagDAO.Instance.GetTagsAsync(_context);
        public Task<Tag?> GetTagByIdAsync(int id) => TagDAO.Instance.GetTagByIdAsync(_context, id);
        public Task<Tag> AddTagAsync(Tag tag) => TagDAO.Instance.AddTagAsync(_context, tag);
        public Task<Tag> UpdateTagAsync(Tag tag) => TagDAO.Instance.UpdateTagAsync(_context, tag);
        public Task<bool> DeleteTagAsync(int id) => TagDAO.Instance.DeleteTagAsync(_context, id);
    }
}
