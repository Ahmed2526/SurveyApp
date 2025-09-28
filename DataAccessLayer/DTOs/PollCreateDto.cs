using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.DTOs
{
    public class PollCreateDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Summary { get; set; }

        [Required]
        public DateOnly StartsAt { get; set; }

        [Required]
        public DateOnly EndsAt { get; set; }
        public bool IsPublished { get; set; }
    }
}
