using FUNewsManagementAPI.DAO;
using FUNewsManagementAPI.Models;
using FUNewsManagementAPI.Repositories.Interfaces;

namespace FUNewsManagementAPI.Repositories.Implementations
{
    public class NewsArticleRepository : INewsArticleRepository
    {
        private readonly FUNewsManagementDbContext _context;

        public NewsArticleRepository(FUNewsManagementDbContext context)
        {
            _context = context;
        }

        public Task<List<NewsArticle>> GetArticlesAsync(bool? activeOnly = null, string? keyword = null, short? categoryId = null, int? tagId = null)
            => NewsArticleDAO.Instance.GetArticlesAsync(_context, activeOnly, keyword, categoryId, tagId);

        public Task<NewsArticle?> GetArticleByIdAsync(string id)
            => NewsArticleDAO.Instance.GetArticleByIdAsync(_context, id);

        public Task<List<NewsArticle>> GetArticlesByAuthorAsync(short authorId)
            => NewsArticleDAO.Instance.GetArticlesByAuthorAsync(_context, authorId);

        public Task<NewsArticle> AddArticleAsync(NewsArticle article, List<int>? tagIds = null)
            => NewsArticleDAO.Instance.AddArticleAsync(_context, article, tagIds);

        public Task<NewsArticle> UpdateArticleAsync(NewsArticle article, List<int>? tagIds = null)
            => NewsArticleDAO.Instance.UpdateArticleAsync(_context, article, tagIds);

        public Task<bool> DeleteArticleAsync(string id)
            => NewsArticleDAO.Instance.DeleteArticleAsync(_context, id);

        public Task<List<NewsArticle>> GetReportAsync(DateTime startDate, DateTime endDate)
            => NewsArticleDAO.Instance.GetReportAsync(_context, startDate, endDate);
    }
}
