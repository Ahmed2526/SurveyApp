using BussinessLogicLater.IService;
using DataAccessLayer.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace SurveyAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> login(LoginDto loginCredentials)
        {
            var result = await _authService.LoginAsync(loginCredentials);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(RegisterDto registerCredentials)
        {
            var result = await _authService.RegisterAsync(registerCredentials);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

    }
}
