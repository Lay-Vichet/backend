using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SubscriptionTracker.Application.DTOs;
using SubscriptionTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace SubscriptionTracker.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var res = await _authService.RegisterAsync(request);
                return CreatedAtAction(null, new { userId = res });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var res = await _authService.LoginAsync(request);
                return Ok(res);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { error = "Invalid credentials" });
            }
        }

        [HttpPost("refresh")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
        {
            try
            {
                var res = await _authService.RefreshAsync(request);
                return Ok(res);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost("revoke")]
        public async Task<IActionResult> Revoke([FromBody] RevokeRequest request)
        {
            await _authService.RevokeRefreshAsync(request);
            return NoContent();
        }
    }
}
