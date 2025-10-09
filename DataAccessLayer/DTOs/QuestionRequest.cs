using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.DTOs
{
    public class QuestionRequest
    {
        [Required]
        [Length(3, 3000)]
        public string Content { get; set; }
        public bool IsActive { get; set; }

        [Required]
        public IEnumerable<string> Answers { get; set; }

    }
}
