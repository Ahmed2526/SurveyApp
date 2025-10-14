using SurveyAppAPI.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.DTOs
{
    public class VoteRequest
    {
        [UniqueAndValidVoteAnswers]
        public IEnumerable<VoteAnswerRequest> Votes { get; set; }
    }
}
