using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.DTOs
{
    public class VoteAnswerRequest
    {
        [Required]
        public int QuestionId { get; set; }

        [Required]
        public int AnswerId { get; set; }
    }
}
