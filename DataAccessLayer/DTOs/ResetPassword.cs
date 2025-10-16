using DataAccessLayer.Errors;
using DataAccessLayer.Patterns;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
