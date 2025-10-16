using DataAccessLayer.Errors;
using DataAccessLayer.Patterns;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.DTOs
{
    public class ChangePasswordRequest
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string OldPassword { get; set; }

        [Required]
        [MaxLength(250)]
        [RegularExpression(RegexPatterns.PasswordPattern, ErrorMessage = RegexErrors.PasswordPattern)]
        public string NewPassword { get; set; }
    }
}
