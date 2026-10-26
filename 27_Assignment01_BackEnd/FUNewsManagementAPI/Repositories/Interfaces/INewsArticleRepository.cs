using FUNewsManagementAPI.Models;

namespace FUNewsManagementAPI.Repositories.Interfaces
{
    public interface INewsArticleRepository
    {
        Task<List<NewsArticle>> GetArticlesAsync(bool? activeOnly = null, string? keyword = null, short? categoryId = null, int? tagId = null);
        Task<NewsArticle?> GetArticleByIdAsync(string id);
        Task<List<NewsArticle>> GetArticlesByAuthorAsync(short authorId);
        Task<NewsArticle> AddArticleAsync(NewsArticle article, List<int>? tagIds = null);
        Task<NewsArticle> UpdateArticleAsync(NewsArticle article, List<int>? tagIds = null);
        Task<bool> DeleteArticleAsync(string id);
        Task<List<NewsArticle>> GetReportAsync(DateTime startDate, DateTime endDate);
    }
}
