using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Models
{
    public class VoteAnswer
    {
        public int Id { get; set; }

        [ForeignKey(nameof(Vote))]
        public int VoteId { get; set; }
        public Vote Vote { get; set; }
        public int QuestionId { get; set; }
        public int AnswerId { get; set; }

    }
}
