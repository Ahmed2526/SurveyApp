namespace DataAccessLayer.Models
{
    public class Poll : AuditEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public bool IsPublished { get; set; }
        public DateOnly StartsAt { get; set; }
        public DateOnly EndsAt { get; set; }

        public IEnumerable<Question> Questions { get; set; }
    }
}
