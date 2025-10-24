using BussinessLogicLater.IService;
using DataAccessLayer.DTOs;
using DataAccessLayer.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SurveyAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.Admin)]
    public class RolesController : ControllerBase
    {
        private readonly IRolesService _rolesService;

        public RolesController(IRolesService rolesService)
        {
            _rolesService = rolesService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var response = await _rolesService.GetAllRolesAsync(cancellationToken);
            return Ok(response);
        }

        [HttpGet]
        [Route("{Id}")]
        public async Task<IActionResult> GetById([FromRoute] string Id, CancellationToken cancellationToken)
        {
            var response = await _rolesService.GetByIdAsync(Id, cancellationToken);

            if (!response.IsSuccess)
                return NotFound();

            return Ok(response);
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Create([FromBody] RoleRequest request, CancellationToken cancellationToken)
        {
            var response = await _rolesService.CreateAsync(request, cancellationToken);

            if (!response.IsSuccess)
                return BadRequest(response);

            return CreatedAtAction("GetById", new { id = response.Data!.RoleId }, response);
        }

        [HttpPut]
        [Route("{Id}")]
        public async Task<IActionResult> Update([FromRoute] string Id, [FromBody] RoleRequest request, CancellationToken cancellationToken)
        {
            var response = await _rolesService.UpdateAsync(Id, request, cancellationToken);

            if (!response.IsSuccess)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpDelete]
        [Route("{Id}")]
        public async Task<IActionResult> ToggleStatus([FromRoute] string Id, CancellationToken cancellationToken)
        {
            var response = await _rolesService.ToggleAsync(Id, cancellationToken);

            if (!response.IsSuccess)
                return NotFound();

            return Ok(response);
        }
    }
}
