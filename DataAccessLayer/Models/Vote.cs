using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Models
{
    public class Vote
    {
        public int Id { get; set; }

        [ForeignKey(nameof(Poll))]
        public int PollId { get; set; }
        public Poll Poll { get; set; }

        [ForeignKey(nameof(ApplicationUser))]
        public string UserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public DateTime SubmittedOn { get; set; }
        public ICollection<VoteAnswer> Answers { get; set; }
    }
}
