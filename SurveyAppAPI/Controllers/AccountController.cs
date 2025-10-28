using BussinessLogicLater.IService;
using DataAccessLayer.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SurveyAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var result = await _accountService.GetUserProfile(cancellationToken);

            if (!result.IsSuccess)
                return Unauthorized(result);

            return Ok(result);
        }

        [HttpPut]
        [Route("")]
        public async Task<IActionResult> UpdateProfile([FromBody] UserProfileRequest request, CancellationToken cancellationToken)
        {
            var result = await _accountService.UpdateUserProfile(request, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Accepted(result);
        }

    }
}
