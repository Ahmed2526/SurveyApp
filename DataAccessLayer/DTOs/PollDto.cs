namespace DataAccessLayer.DTOs
{
    public class PollDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public bool IsPublished { get; set; }
        public DateOnly StartsAt { get; set; }
        public DateOnly EndsAt { get; set; }

        public string CreatedById { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public string? UpdatedById { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
