using BussinessLogicLater.IService;
using DataAccessLayer.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SurveyAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PollsController : ControllerBase
    {
        private readonly IPollsService _pollsService;

        public PollsController(IPollsService pollsService)
        {
            _pollsService = pollsService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var data = await _pollsService.GetAllAsync(cancellationToken);

            return Ok(data);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var data = await _pollsService.GetByIdAsync(id, cancellationToken);

            if (data.IsSuccess)
                return Ok(data);

            return NotFound(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PollCreateDto dto, CancellationToken cancellationToken)
        {
            var data = await _pollsService.CreateAsync(dto, cancellationToken);

            if (data.IsSuccess)
                return CreatedAtAction("GetById", new { id = data.Data!.Id }, data);

            return BadRequest(data);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromQuery] int id, PollCreateDto dto, CancellationToken cancellationToken)
        {
            var data = await _pollsService.UpdateAsync(id, dto, cancellationToken);

            if (data.IsSuccess)
                return Ok(data);

            return BadRequest(data);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] int id, CancellationToken cancellationToken)
        {
            var data = await _pollsService.DeleteAsync(id, cancellationToken);

            if (data.IsSuccess)
                return Ok(data);

            return NotFound(data);
        }


    }
}
