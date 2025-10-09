namespace DataAccessLayer.DTOs
{
    public class QuestionResponse
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public bool IsActive { get; set; }
        public int PollId { get; set; }

        public List<string> Answers { get; set; }
    }
}
