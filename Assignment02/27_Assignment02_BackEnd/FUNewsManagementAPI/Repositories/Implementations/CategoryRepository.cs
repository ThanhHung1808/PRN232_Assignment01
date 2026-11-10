using FUNewsManagementAPI.DAO;
using FUNewsManagementAPI.Models;
using FUNewsManagementAPI.Repositories.Interfaces;

namespace FUNewsManagementAPI.Repositories.Implementations
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly FUNewsManagementDbContext _context;

        public CategoryRepository(FUNewsManagementDbContext context)
        {
            _context = context;
        }

        public Task<List<Category>> GetCategoriesAsync(bool? activeOnly = null) 
            => CategoryDAO.Instance.GetCategoriesAsync(_context, activeOnly);

        public Task<Category?> GetCategoryByIdAsync(short id) 
            => CategoryDAO.Instance.GetCategoryByIdAsync(_context, id);

        public Task<Category> AddCategoryAsync(Category category) 
            => CategoryDAO.Instance.AddCategoryAsync(_context, category);

        public Task<Category> UpdateCategoryAsync(Category category) 
            => CategoryDAO.Instance.UpdateCategoryAsync(_context, category);

        public Task<bool> DeleteCategoryAsync(short id) 
            => CategoryDAO.Instance.DeleteCategoryAsync(_context, id);

        public Task<bool> HasNewsArticlesAsync(short categoryId) 
            => CategoryDAO.Instance.HasNewsArticlesAsync(_context, categoryId);
    }
}
