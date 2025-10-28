using BussinessLogicLater.IService;
using DataAccessLayer.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SurveyAppAPI.Controllers
{
    [Route("api/Polls/{PollId}/[controller]")]
    [ApiController]
    [Authorize]
    public class QuestionsController : ControllerBase
    {
        private readonly IQuestionService _questionService;

        public QuestionsController(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromRoute] int PollId, [FromQuery] FilterRequest filterRequest, CancellationToken cancellationToken)
        {
            var result = await _questionService.GetAllAsync(PollId, filterRequest, cancellationToken);

            if (result.IsSuccess)
                return Ok(result);

            return NotFound(result);
        }

        [HttpGet]
        [Route("{QuestionId}")]
        public async Task<IActionResult> GetById([FromRoute] int PollId, [FromRoute] int QuestionId, CancellationToken cancellationToken)
        {
            var result = await _questionService.GetByIdAsync(PollId, QuestionId, cancellationToken);

            if (result.IsSuccess)
                return Ok(result);

            return NotFound(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromRoute] int PollId, QuestionRequest request, CancellationToken cancellationToken)
        {
            var result = await _questionService.CreateAsync(PollId, request, cancellationToken);

            if (result.IsSuccess)
                return CreatedAtAction(nameof(GetById), new { PollId = PollId, QuestionId = result.Data!.Id }, result);

            return BadRequest(result);
        }

        [HttpPut]
        [Route("{QuestionId}")]
        public async Task<IActionResult> Update([FromRoute] int PollId, [FromRoute] int QuestionId, QuestionRequest request, CancellationToken cancellationToken)
        {
            var result = await _questionService.UpdateAsync(PollId, QuestionId, request, cancellationToken);

            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);

        }

        [HttpDelete]
        [Route("{QuestionId}")]
        public async Task<IActionResult> Delete([FromRoute] int PollId, [FromRoute] int QuestionId, CancellationToken cancellationToken)
        {
            var result = await _questionService.DeleteAsync(PollId, QuestionId, cancellationToken);

            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);

        }

    }
}
