using System.ComponentModel.DataAnnotations;

namespace FUNewsManagementClient.DTOs
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public short AccountId { get; set; }
        public string AccountName { get; set; } = string.Empty;
        public string AccountEmail { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int? AccountRole { get; set; }
        public string Token { get; set; } = string.Empty;
    }

    public class CategoryCreateUpdateDto
    {
        public short CategoryId { get; set; }

        [Required(ErrorMessage = "Category Name is required")]
        [StringLength(100, ErrorMessage = "Category Name cannot exceed 100 characters")]
        public string CategoryName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category Description is required")]
        [StringLength(250, ErrorMessage = "Category Description cannot exceed 250 characters")]
        public string CategoryDesciption { get; set; } = string.Empty;

        public short? ParentCategoryId { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class CategoryResponseDto
    {
        public short CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string CategoryDesciption { get; set; } = string.Empty;
        public short? ParentCategoryId { get; set; }
        public string? ParentCategoryName { get; set; }
        public bool? IsActive { get; set; }
        public int ArticleCount { get; set; }
    }

    public class NewsArticleCreateUpdateDto
    {
        [Required(ErrorMessage = "News Article ID is required")]
        [StringLength(20, ErrorMessage = "News Article ID cannot exceed 20 characters")]
        public string NewsArticleId { get; set; } = string.Empty;

        [StringLength(400, ErrorMessage = "News Title cannot exceed 400 characters")]
        public string? NewsTitle { get; set; }

        [Required(ErrorMessage = "Headline is required")]
        [StringLength(150, ErrorMessage = "Headline cannot exceed 150 characters")]
        public string Headline { get; set; } = string.Empty;

        [StringLength(4000, ErrorMessage = "News Content cannot exceed 4000 characters")]
        public string? NewsContent { get; set; }

        [StringLength(400, ErrorMessage = "News Source cannot exceed 400 characters")]
        public string? NewsSource { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public short CategoryId { get; set; }

        public bool NewsStatus { get; set; } = true;
        public List<int> TagIds { get; set; } = new List<int>();
    }

    public class NewsArticleResponseDto
    {
        public string NewsArticleId { get; set; } = string.Empty;
        public string? NewsTitle { get; set; }
        public string Headline { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }
        public string? NewsContent { get; set; }
        public string? NewsSource { get; set; }
        public short? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public bool? NewsStatus { get; set; }
        public short? CreatedById { get; set; }
        public string? CreatedByName { get; set; }
        public short? UpdatedById { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public List<TagDto> Tags { get; set; } = new List<TagDto>();
    }

    public class TagDto
    {
        public int TagId { get; set; }
        public string? TagName { get; set; }
        public string? Note { get; set; }
    }

    public class AccountCreateUpdateDto
    {
        [Required(ErrorMessage = "Account ID is required")]
        public short AccountId { get; set; }

        [Required(ErrorMessage = "Account Name is required")]
        [StringLength(100, ErrorMessage = "Account Name cannot exceed 100 characters")]
        public string AccountName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Account Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email address")]
        [StringLength(70, ErrorMessage = "Email cannot exceed 70 characters")]
        public string AccountEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required")]
        public int AccountRole { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(70, MinimumLength = 1, ErrorMessage = "Password must be up to 70 characters")]
        public string AccountPassword { get; set; } = string.Empty;
    }

    public class AccountResponseDto
    {
        public short AccountId { get; set; }
        public string? AccountName { get; set; }
        public string? AccountEmail { get; set; }
        public int? AccountRole { get; set; }
        public string RoleName => AccountRole == 1 ? "Staff" : AccountRole == 2 ? "Lecturer" : "Unknown";
        public int CreatedArticlesCount { get; set; }
    }

    public class ProfileUpdateDto
    {
        [Required(ErrorMessage = "Account Name is required")]
        [StringLength(100)]
        public string AccountName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Account Email is required")]
        [EmailAddress]
        [StringLength(70)]
        public string AccountEmail { get; set; } = string.Empty;

        [StringLength(70)]
        public string? NewPassword { get; set; }
    }

    public class ReportStatisticDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalArticles { get; set; }
        public List<NewsArticleResponseDto> Articles { get; set; } = new List<NewsArticleResponseDto>();
    }
}
