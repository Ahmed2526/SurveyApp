using DataAccessLayer.Errors;
using DataAccessLayer.Patterns;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.DTOs
{
    public class ResetPassword
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        [MaxLength(250)]
        [RegularExpression(RegexPatterns.PasswordPattern, ErrorMessage = RegexErrors.PasswordPattern)]
        public string NewPassword { get; set; }

    }
}
