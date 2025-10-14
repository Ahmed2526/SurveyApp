using BussinessLogicLater.IService;
using DataAccessLayer.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace SurveyAppAPI.Controllers
{
    [Route("api/Polls/{PollId}/Vote")]
    [ApiController]
    [Authorize]
    public class VotesController : ControllerBase
    {
        private readonly IVotesService _votesService;

        public VotesController(IVotesService votesService)
        {
            _votesService = votesService;
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
