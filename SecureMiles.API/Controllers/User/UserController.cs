using Microsoft.AspNetCore.Mvc;
using SecureMiles.Services;
using SecureMiles.Common.DTOs;
using Microsoft.AspNetCore.Authorization;
using Serilog;


namespace SecureMiles.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }





        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] SignInRequestDto signInRequest)
        {
            _logger.LogInformation("User attempting to sign in with email: {Email}", signInRequest.Email);
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid sign-in request: {Request}", signInRequest);
                return BadRequest(ModelState);
            }

            try
            {
                var response = await _userService.SignInAsync(signInRequest);
                _logger.LogInformation("User signed in successfully: {Email}", signInRequest.Email);
                return Ok(response); // 200 OK with token
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Unauthorized sign-in attempt: {Email}", signInRequest.Email);
                return Unauthorized(new { Error = ex.Message }); // 401 Unauthorized
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while signing in: {Email}", signInRequest.Email);
                return StatusCode(500, new { Error = "An unexpected error occurred.", Details = ex.Message }); // 500 Internal Server Error
            }
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequest)
        {
            _logger.LogInformation("User attempting to register with email: {Email}", registerRequest.Email);
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid registration request: {Request}", registerRequest);
                return BadRequest(ModelState); // 400 Bad Request for invalid input
            }

            try
            {
                var result = await _userService.RegisterUserAsync(registerRequest);
                _logger.LogInformation("User registered successfully: {Email}", registerRequest.Email);
                return Ok(new { Message = result }); // 200 OK
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("User registration failed: {Email}", registerRequest.Email);
                return Conflict(new { Error = ex.Message }); // 409 Conflict if email exists
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while registering: {Email}", registerRequest.Email);
                return StatusCode(500, new { Error = "An unexpected error occurred.", Details = ex.Message }); // 500 Internal Server Error
            }
        }

    }
}
