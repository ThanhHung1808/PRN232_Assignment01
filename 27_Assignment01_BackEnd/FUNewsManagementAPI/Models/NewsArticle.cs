using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FUNewsManagementAPI.Models
{
    [Table("NewsArticle")]
    public class NewsArticle
    {
        [Key]
        [MaxLength(20)]
        public string NewsArticleId { get; set; } = null!;

        [MaxLength(400)]
        public string? NewsTitle { get; set; }

        [Required]
        [MaxLength(150)]
        public string Headline { get; set; } = null!;

        public DateTime? CreatedDate { get; set; }

        [MaxLength(4000)]
        public string? NewsContent { get; set; }

        [MaxLength(400)]
        public string? NewsSource { get; set; }

        public short? CategoryId { get; set; }

        public bool? NewsStatus { get; set; }

        public short? CreatedById { get; set; }

        public short? UpdatedById { get; set; }

        public DateTime? ModifiedDate { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }

        [ForeignKey("CreatedById")]
        public virtual SystemAccount? CreatedBy { get; set; }

        public virtual ICollection<NewsTag> NewsTags { get; set; } = new List<NewsTag>();
    }
}
