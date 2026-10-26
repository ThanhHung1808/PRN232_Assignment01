using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FUNewsManagementAPI.Models
{
    [Table("NewsTag")]
    public class NewsTag
    {
        [Key, Column(Order = 0)]
        [MaxLength(20)]
        public string NewsArticleId { get; set; } = null!;

        [Key, Column(Order = 1)]
        public int TagId { get; set; }

        [ForeignKey("NewsArticleId")]
        [JsonIgnore]
        public virtual NewsArticle NewsArticle { get; set; } = null!;

        [ForeignKey("TagId")]
        public virtual Tag Tag { get; set; } = null!;
    }
}
