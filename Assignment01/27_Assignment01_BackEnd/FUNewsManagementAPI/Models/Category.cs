using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FUNewsManagementAPI.Models
{
    [Table("Category")]
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public short CategoryId { get; set; }

        [Required]
        [MaxLength(100)]
        public string CategoryName { get; set; } = null!;

        [Required]
        [MaxLength(250)]
        public string CategoryDesciption { get; set; } = null!;

        public short? ParentCategoryId { get; set; }

        public bool? IsActive { get; set; } = true;

        [ForeignKey("ParentCategoryId")]
        public virtual Category? ParentCategory { get; set; }

        [JsonIgnore]
        public virtual ICollection<Category> SubCategories { get; set; } = new List<Category>();

        [JsonIgnore]
        public virtual ICollection<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();
    }
}
