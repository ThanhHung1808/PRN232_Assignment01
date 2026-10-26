using FUNewsManagementAPI.DAO;
using FUNewsManagementAPI.Models;
using FUNewsManagementAPI.Repositories.Interfaces;

namespace FUNewsManagementAPI.Repositories.Implementations
{
    public class NewsArticleRepository : INewsArticleRepository
    {
        public Task<List<NewsArticle>> GetArticlesAsync(bool? activeOnly = null, string? keyword = null, short? categoryId = null, int? tagId = null)
            => NewsArticleDAO.Instance.GetArticlesAsync(activeOnly, keyword, categoryId, tagId);

        public Task<NewsArticle?> GetArticleByIdAsync(string id)
            => NewsArticleDAO.Instance.GetArticleByIdAsync(id);

        public Task<List<NewsArticle>> GetArticlesByAuthorAsync(short authorId)
            => NewsArticleDAO.Instance.GetArticlesByAuthorAsync(authorId);

        public Task<NewsArticle> AddArticleAsync(NewsArticle article, List<int>? tagIds = null)
            => NewsArticleDAO.Instance.AddArticleAsync(article, tagIds);

        public Task<NewsArticle> UpdateArticleAsync(NewsArticle article, List<int>? tagIds = null)
            => NewsArticleDAO.Instance.UpdateArticleAsync(article, tagIds);

        public Task<bool> DeleteArticleAsync(string id)
            => NewsArticleDAO.Instance.DeleteArticleAsync(id);

        public Task<List<NewsArticle>> GetReportAsync(DateTime startDate, DateTime endDate)
            => NewsArticleDAO.Instance.GetReportAsync(startDate, endDate);
    }
}
