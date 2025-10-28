using BussinessLogicLater.IService;
using DataAccessLayer.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace SurveyAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var data = await _userService.GetAllAsync(cancellationToken);

            return Ok(data);
        }

        [HttpGet]
        [Route("{Id}")]
        public async Task<IActionResult> Get([FromRoute] string Id, CancellationToken cancellationToken)
        {
            var result = await _userService.GetByIdAsync(Id, cancellationToken);

            if (!result.IsSuccess)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Create([FromBody] UserRequest request, CancellationToken cancellationToken)
        {
            var result = await _userService.CreateAsync(request, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost]
        [Route("{Id}")]
        public async Task<IActionResult> Edit([FromRoute] string Id, [FromBody] List<string> Roles, CancellationToken cancellationToken)
        {
            var result = await _userService.EditUserRolesAsync(Id, Roles, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost]
        [Route("lockout/{Id}")]
        public async Task<IActionResult> ToggleLockout([FromRoute] string Id, CancellationToken cancellationToken)
        {
            var result = await _userService.ToggleLockoutAsync(Id, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

    }
}
