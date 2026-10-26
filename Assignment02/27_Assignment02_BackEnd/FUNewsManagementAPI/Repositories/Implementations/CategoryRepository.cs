using FUNewsManagementAPI.DAO;
using FUNewsManagementAPI.Models;
using FUNewsManagementAPI.Repositories.Interfaces;

namespace FUNewsManagementAPI.Repositories.Implementations
{
    public class CategoryRepository : ICategoryRepository
    {
        public Task<List<Category>> GetCategoriesAsync(bool? activeOnly = null) 
            => CategoryDAO.Instance.GetCategoriesAsync(activeOnly);

        public Task<Category?> GetCategoryByIdAsync(short id) 
            => CategoryDAO.Instance.GetCategoryByIdAsync(id);

        public Task<Category> AddCategoryAsync(Category category) 
            => CategoryDAO.Instance.AddCategoryAsync(category);

        public Task<Category> UpdateCategoryAsync(Category category) 
            => CategoryDAO.Instance.UpdateCategoryAsync(category);

        public Task<bool> DeleteCategoryAsync(short id) 
            => CategoryDAO.Instance.DeleteCategoryAsync(id);

        public Task<bool> HasNewsArticlesAsync(short categoryId) 
            => CategoryDAO.Instance.HasNewsArticlesAsync(categoryId);
    }
}
