using Microsoft.AspNetCore.Mvc;
using SecureMiles.Services;
using SecureMiles.Common.DTOs;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using System.Security.Claims;
using SecureMiles.Common.DTOs.User;


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
                _logger.LogError(ex, "An unexpected error occurred while reg--->: {Email}", registerRequest.Email);
                return StatusCode(500, new { Error = "An unexpected error occurred.", Details = ex.Message }); // 500 Internal Server Error
            }
        }





        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Validation error
            }

            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null)
                {
                    _logger.LogWarning("User ID claim not found.");
                    return Unauthorized(new { Error = "User ID claim not found." });
                }
                var userId = int.Parse(userIdClaim); // Extract UserID from JWT
                var result = await _userService.UpdateUserProfileAsync(userId, request);

                _logger.LogInformation("User {UserId} updated their profile successfully.", userId);

                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "User not found for profile update.");
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while updating profile.");
                return StatusCode(500, new { Message = "An unexpected error occurred." });
            }
        }

        [HttpGet("profile")]
        [Authorize] // Ensures only authenticated users can access this
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                // Extract UserID from JWT claims
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null)
                {
                    _logger.LogWarning("User ID claim not found.");
                    return Unauthorized(new { Error = "User ID claim not found." });
                }
                var userId = int.Parse(userIdClaim);

                // Fetch the user's profile
                var userProfile = await _userService.GetUserProfileAsync(userId);

                return Ok(userProfile);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Profile retrieval failed for user ID {UserId}.", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return NotFound(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while retrieving the profile for user ID {UserId}.", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return StatusCode(500, new { Error = "An unexpected error occurred." });
            }
        }
        [HttpDelete("{userId}")]
        [Authorize(Roles = "Admin")] // Restrict to admins
        public async Task<IActionResult> DeleteUser(int userId)
        {
            try
            {
                await _userService.DeleteUserAsync(userId);
                _logger.LogInformation("User {UserId} deleted successfully by Admin.", userId);
                return NoContent(); // 204 No Content
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Failed to delete user. User {UserId} not found.", userId);
                return NotFound(new { Error = ex.Message }); // 404 Not Found
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while deleting user {UserId}.", userId);
                return StatusCode(500, new { Error = "An unexpected error occurred." }); // 500 Internal Server Error
            }
        }
        [HttpDelete("profile")]
        [Authorize] // Ensure the user is logged in
        public async Task<IActionResult> DeleteOwnProfile()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null)
                {
                    _logger.LogWarning("User ID claim not found.");
                    return Unauthorized(new { Error = "User ID claim not found." });
                }
                var userId = int.Parse(userIdClaim); // Extract UserID from JWT
                await _userService.DeleteUserAsync(userId);

                _logger.LogInformation("User {UserId} deleted their own profile.", userId);

                // return message
                return Ok(new { Message = "User profile deleted successfully." });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while deleting profile for user {UserId}.", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return StatusCode(500, new { Error = "An unexpected error occurred." }); // 500 Internal Server Error
            }
        }


    }
}
