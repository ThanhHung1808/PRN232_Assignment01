using FUNewsManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagementAPI.DAO
{
    public class NewsArticleDAO
    {
        private static NewsArticleDAO? _instance;
        private static readonly object _lock = new object();

        private NewsArticleDAO() { }

        public static NewsArticleDAO Instance
        {
            get
            {
                lock (_lock)
                {
                    return _instance ??= new NewsArticleDAO();
                }
            }
        }

        public async Task<List<NewsArticle>> GetArticlesAsync(bool? activeOnly = null, string? keyword = null, short? categoryId = null, int? tagId = null)
        {
            using var context = new FUNewsManagementDbContext();
            var query = context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.NewsTags)
                    .ThenInclude(nt => nt.Tag)
                .AsQueryable();

            if (activeOnly.HasValue && activeOnly.Value)
            {
                query = query.Where(n => n.NewsStatus == true);
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var lower = keyword.Trim().ToLower();
                query = query.Where(n => (n.NewsTitle != null && n.NewsTitle.ToLower().Contains(lower))
                                      || (n.Headline != null && n.Headline.ToLower().Contains(lower))
                                      || (n.NewsContent != null && n.NewsContent.ToLower().Contains(lower)));
            }

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(n => n.CategoryId == categoryId.Value);
            }

            if (tagId.HasValue && tagId.Value > 0)
            {
                query = query.Where(n => n.NewsTags.Any(nt => nt.TagId == tagId.Value));
            }

            return await query.OrderByDescending(n => n.CreatedDate).ToListAsync();
        }

        public async Task<NewsArticle?> GetArticleByIdAsync(string id)
        {
            using var context = new FUNewsManagementDbContext();
            return await context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.NewsTags)
                    .ThenInclude(nt => nt.Tag)
                .FirstOrDefaultAsync(n => n.NewsArticleId == id);
        }

        public async Task<List<NewsArticle>> GetArticlesByAuthorAsync(short authorId)
        {
            using var context = new FUNewsManagementDbContext();
            return await context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.NewsTags)
                    .ThenInclude(nt => nt.Tag)
                .Where(n => n.CreatedById == authorId)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
        }

        public async Task<NewsArticle> AddArticleAsync(NewsArticle article, List<int>? tagIds = null)
        {
            using var context = new FUNewsManagementDbContext();

            // 1. Check duplicate NewsArticleId
            var existing = await context.NewsArticles.FindAsync(article.NewsArticleId);
            if (existing != null)
            {
                throw new InvalidOperationException($"News Article with ID '{article.NewsArticleId}' already exists.");
            }

            // 2. Check Category existence
            if (article.CategoryId.HasValue)
            {
                var catExists = await context.Categories.AnyAsync(c => c.CategoryId == article.CategoryId.Value);
                if (!catExists)
                {
                    throw new InvalidOperationException($"Category ID {article.CategoryId.Value} does not exist.");
                }
            }
            else
            {
                throw new InvalidOperationException("Category is required for a news article.");
            }

            article.CreatedDate ??= DateTime.Now;
            article.ModifiedDate ??= DateTime.Now;

            context.NewsArticles.Add(article);
            await context.SaveChangesAsync();

            if (tagIds != null && tagIds.Count > 0)
            {
                foreach (var tagId in tagIds.Distinct())
                {
                    var tagExists = await context.Tags.AnyAsync(t => t.TagId == tagId);
                    if (tagExists)
                    {
                        context.NewsTags.Add(new NewsTag
                        {
                            NewsArticleId = article.NewsArticleId,
                            TagId = tagId
                        });
                    }
                }
                await context.SaveChangesAsync();
            }

            return await GetArticleByIdAsync(article.NewsArticleId) ?? article;
        }

        public async Task<NewsArticle> UpdateArticleAsync(NewsArticle article, List<int>? tagIds = null)
        {
            using var context = new FUNewsManagementDbContext();
            var existing = await context.NewsArticles
                .Include(n => n.NewsTags)
                .FirstOrDefaultAsync(n => n.NewsArticleId == article.NewsArticleId);

            if (existing == null)
            {
                throw new KeyNotFoundException($"News Article with ID '{article.NewsArticleId}' not found.");
            }

            // Check Category existence
            if (article.CategoryId.HasValue)
            {
                var catExists = await context.Categories.AnyAsync(c => c.CategoryId == article.CategoryId.Value);
                if (!catExists)
                {
                    throw new InvalidOperationException($"Category ID {article.CategoryId.Value} does not exist.");
                }
                existing.CategoryId = article.CategoryId.Value;
            }

            existing.NewsTitle = article.NewsTitle;
            existing.Headline = article.Headline;
            existing.NewsContent = article.NewsContent;
            existing.NewsSource = article.NewsSource;
            existing.NewsStatus = article.NewsStatus;
            existing.UpdatedById = article.UpdatedById;
            existing.ModifiedDate = DateTime.Now;

            if (tagIds != null)
            {
                var toRemove = existing.NewsTags.Where(nt => !tagIds.Contains(nt.TagId)).ToList();
                context.NewsTags.RemoveRange(toRemove);

                var currentTagIds = existing.NewsTags.Select(nt => nt.TagId).ToList();
                var toAdd = tagIds.Distinct().Where(tid => !currentTagIds.Contains(tid));
                foreach (var tid in toAdd)
                {
                    var tagExists = await context.Tags.AnyAsync(t => t.TagId == tid);
                    if (tagExists)
                    {
                        context.NewsTags.Add(new NewsTag
                        {
                            NewsArticleId = existing.NewsArticleId,
                            TagId = tid
                        });
                    }
                }
            }

            await context.SaveChangesAsync();
            return await GetArticleByIdAsync(existing.NewsArticleId) ?? existing;
        }

        public async Task<bool> DeleteArticleAsync(string id)
        {
            using var context = new FUNewsManagementDbContext();
            var existing = await context.NewsArticles
                .Include(n => n.NewsTags)
                .FirstOrDefaultAsync(n => n.NewsArticleId == id);

            if (existing == null) return false;

            if (existing.NewsTags.Any())
            {
                context.NewsTags.RemoveRange(existing.NewsTags);
            }

            context.NewsArticles.Remove(existing);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<List<NewsArticle>> GetReportAsync(DateTime startDate, DateTime endDate)
        {
            using var context = new FUNewsManagementDbContext();
            var start = startDate.Date;
            var end = endDate.Date.AddDays(1).AddTicks(-1);

            return await context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.NewsTags)
                    .ThenInclude(nt => nt.Tag)
                .Where(n => n.CreatedDate >= start && n.CreatedDate <= end)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
        }
    }
}
