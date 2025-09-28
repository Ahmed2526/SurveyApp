namespace DataAccessLayer.Models
{
    public class AuditEntity
    {
        public string CreatedById { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public string? UpdatedById { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }

}
