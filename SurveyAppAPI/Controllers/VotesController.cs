using BussinessLogicLater.IService;
using DataAccessLayer.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SurveyAppAPI.Controllers
{
    [Route("api/Polls/{PollId}/Vote")]
    [ApiController]
    [Authorize]
    public class VotesController : ControllerBase
    {
        private readonly IVotesService _votesService;
        private readonly IQuestionService _questionService;

        public VotesController(IVotesService votesService, IQuestionService questionService)
        {
            _votesService = votesService;
            _questionService = questionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPoll([FromRoute] int PollId, CancellationToken cancellationToken)
        {
            var response = await _questionService.GetAvailableAsync(PollId, cancellationToken);

            if (response.IsSuccess)
                return Ok(response);

            return BadRequest(response);
        }

        [HttpPost]
        public async Task<IActionResult> Vote([FromRoute] int PollId, VoteRequest voteRequest, CancellationToken cancellationToken)
        {
            var response = await _votesService.Vote(PollId, voteRequest, cancellationToken);

            if (response.IsSuccess)
                return Ok(response);

            return BadRequest(response);
        }
    }
}
