using DataAccessLayer.Errors;
using DataAccessLayer.Patterns;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.DTOs
{
    public class UserRequest
    {
        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        [MaxLength(100)]
        public string UserName { get; set; }

        [Required]
        [RegularExpression(RegexPatterns.EgyPhonePattern, ErrorMessage = RegexErrors.PhonePattern)]
        public string Phone { get; set; }

        [Required]
        [MaxLength(250)]
        [RegularExpression(RegexPatterns.PasswordPattern, ErrorMessage = RegexErrors.PasswordPattern)]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = RegexErrors.PasswordMisMatch)]
        public string ConfirmPassword { get; set; }

        public List<string> Roles { get; set; }
    }
}
