using System.ComponentModel.DataAnnotations;

namespace FUNewsManagementAPI.DTOs
{
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
}
