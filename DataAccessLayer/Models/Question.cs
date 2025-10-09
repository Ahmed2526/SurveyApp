using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Models
{
    public class Question : AuditEntity
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public bool IsActive { get; set; }

        [ForeignKey(nameof(Poll))]
        public int PollId { get; set; }
        public Poll Poll { get; set; }

        public ICollection<Answer> Answers { get; set; } = new HashSet<Answer>();
    }
}
