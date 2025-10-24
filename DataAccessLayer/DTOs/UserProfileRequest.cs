using DataAccessLayer.Errors;
using DataAccessLayer.Patterns;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.DTOs
{
    public class UserProfileRequest
    {
        [Required]
        [RegularExpression(RegexPatterns.EgyPhonePattern, ErrorMessage = RegexErrors.PhonePattern)]
        public string Phone { get; set; }
    }
}
