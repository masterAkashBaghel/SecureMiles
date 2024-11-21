using Microsoft.AspNetCore.Mvc;
using SecureMiles.Services;
using SecureMiles.Common.DTOs;

namespace SecureMiles.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] SignInRequestDto signInRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var response = await _userService.SignInAsync(signInRequest);
                return Ok(response); // 200 OK with token
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Error = ex.Message }); // 401 Unauthorized
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "An unexpected error occurred.", Details = ex.Message }); // 500 Internal Server Error
            }
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // 400 Bad Request for invalid input
            }

            try
            {
                var result = await _userService.RegisterUserAsync(registerRequest);
                return Ok(new { Message = result }); // 200 OK
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { Error = ex.Message }); // 409 Conflict if email exists
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "An unexpected error occurred.", Details = ex.Message }); // 500 Internal Server Error
            }
        }

    }
}
