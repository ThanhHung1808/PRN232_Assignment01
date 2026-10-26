using FUNewsManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagementAPI.DAO
{
    public class CategoryDAO
    {
        private static CategoryDAO? _instance;
        private static readonly object _lock = new object();

        private CategoryDAO() { }

        public static CategoryDAO Instance
        {
            get
            {
                lock (_lock)
                {
                    return _instance ??= new CategoryDAO();
                }
            }
        }

        public async Task<List<Category>> GetCategoriesAsync(bool? activeOnly = null)
        {
            using var context = new FUNewsManagementDbContext();
            var query = context.Categories
                .Include(c => c.ParentCategory)
                .Include(c => c.NewsArticles)
                .AsQueryable();

            if (activeOnly.HasValue && activeOnly.Value)
            {
                query = query.Where(c => c.IsActive == true);
            }

            return await query.ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(short id)
        {
            using var context = new FUNewsManagementDbContext();
            return await context.Categories
                .Include(c => c.ParentCategory)
                .Include(c => c.NewsArticles)
                .FirstOrDefaultAsync(c => c.CategoryId == id);
        }

        public async Task<Category> AddCategoryAsync(Category category)
        {
            using var context = new FUNewsManagementDbContext();

            // Check duplicate CategoryName
            var nameExists = await context.Categories
                .AnyAsync(c => c.CategoryName.Trim().ToLower() == category.CategoryName.Trim().ToLower());
            if (nameExists)
            {
                throw new InvalidOperationException($"Category name '{category.CategoryName}' already exists.");
            }

            if (category.ParentCategoryId.HasValue && category.ParentCategoryId.Value > 0)
            {
                var parentExists = await context.Categories.AnyAsync(c => c.CategoryId == category.ParentCategoryId.Value);
                if (!parentExists)
                {
                    throw new InvalidOperationException($"Parent Category ID {category.ParentCategoryId.Value} does not exist.");
                }
            }
            else
            {
                category.ParentCategoryId = null;
            }

            context.Categories.Add(category);
            await context.SaveChangesAsync();
            return category;
        }

        public async Task<Category> UpdateCategoryAsync(Category category)
        {
            using var context = new FUNewsManagementDbContext();
            var existing = await context.Categories.FindAsync(category.CategoryId);
            if (existing == null)
            {
                throw new KeyNotFoundException($"Category with ID {category.CategoryId} not found.");
            }

            // Check duplicate CategoryName (excluding current)
            var nameExists = await context.Categories
                .AnyAsync(c => c.CategoryId != category.CategoryId && c.CategoryName.Trim().ToLower() == category.CategoryName.Trim().ToLower());
            if (nameExists)
            {
                throw new InvalidOperationException($"Category name '{category.CategoryName}' is already in use by another category.");
            }

            // Prevent self-referencing parent
            if (category.ParentCategoryId.HasValue && category.ParentCategoryId.Value == category.CategoryId)
            {
                throw new InvalidOperationException("A category cannot be its own parent category.");
            }

            if (category.ParentCategoryId.HasValue && category.ParentCategoryId.Value > 0)
            {
                var parentExists = await context.Categories.AnyAsync(c => c.CategoryId == category.ParentCategoryId.Value);
                if (!parentExists)
                {
                    throw new InvalidOperationException($"Parent Category ID {category.ParentCategoryId.Value} does not exist.");
                }
                existing.ParentCategoryId = category.ParentCategoryId.Value;
            }
            else
            {
                existing.ParentCategoryId = null;
            }

            existing.CategoryName = category.CategoryName;
            existing.CategoryDesciption = category.CategoryDesciption;
            existing.IsActive = category.IsActive;

            await context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> HasNewsArticlesAsync(short categoryId)
        {
            using var context = new FUNewsManagementDbContext();
            return await context.NewsArticles.AnyAsync(n => n.CategoryId == categoryId);
        }

        public async Task<bool> DeleteCategoryAsync(short id)
        {
            using var context = new FUNewsManagementDbContext();
            var existing = await context.Categories.FindAsync(id);
            if (existing == null) return false;

            // Constraint 1: Cannot delete if it has news articles
            var hasArticles = await context.NewsArticles.AnyAsync(n => n.CategoryId == id);
            if (hasArticles)
            {
                throw new InvalidOperationException("Cannot delete this category because it contains news articles.");
            }

            // Constraint 2: Cannot delete if it is parent to other subcategories
            var hasSubCategories = await context.Categories.AnyAsync(c => c.ParentCategoryId == id);
            if (hasSubCategories)
            {
                throw new InvalidOperationException("Cannot delete this category because it has sub-categories linked to it.");
            }

            context.Categories.Remove(existing);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
