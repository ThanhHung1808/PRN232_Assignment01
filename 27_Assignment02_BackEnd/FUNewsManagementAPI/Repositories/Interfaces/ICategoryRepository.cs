using FUNewsManagementAPI.Models;

namespace FUNewsManagementAPI.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetCategoriesAsync(bool? activeOnly = null);
        Task<Category?> GetCategoryByIdAsync(short id);
        Task<Category> AddCategoryAsync(Category category);
        Task<Category> UpdateCategoryAsync(Category category);
        Task<bool> DeleteCategoryAsync(short id);
        Task<bool> HasNewsArticlesAsync(short categoryId);
    }
}
