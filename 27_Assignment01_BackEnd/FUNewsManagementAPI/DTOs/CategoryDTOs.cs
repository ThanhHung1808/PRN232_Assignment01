using System.ComponentModel.DataAnnotations;

namespace FUNewsManagementAPI.DTOs
{
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

        public bool? IsActive { get; set; } = true;
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
}
