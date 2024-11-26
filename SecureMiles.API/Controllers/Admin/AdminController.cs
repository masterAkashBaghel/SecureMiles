using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecureMiles.Common.DTOs.Admin;
using SecureMiles.Services.Admin;

namespace SecureMiles.API.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]

    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IAdminService adminService, ILogger<AdminController> logger)
        {
            _adminService = adminService;
            _logger = logger;
        }

        [HttpGet]
        [Route("api/admin/users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _adminService.GetAllUsersAsync(pageNumber, pageSize);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving users.");
                return StatusCode(500, new { Error = "An unexpected error occurred." });
            }
        }
        [HttpGet("api/admin/users/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserDetails(int userId)
        {
            try
            {
                var userDetails = await _adminService.GetUserDetailsAsync(userId);
                if (userDetails == null)
                {
                    return NotFound(new { Error = "User not found." });
                }

                return Ok(userDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching user details for UserID {UserId}.", userId);
                return StatusCode(500, new { Error = "An unexpected error occurred." });
            }
        }
        [HttpPut("{userId}/role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUserRole(int userId, [FromBody] UpdateUserRoleRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var nameIdentifierClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(nameIdentifierClaim))
                {
                    return Unauthorized(new { Error = "Invalid token: Admin ID not found." });
                }
                var adminId = int.Parse(nameIdentifierClaim); // Extract Admin ID from JWT
                var response = await _adminService.UpdateUserRoleAsync(adminId, userId, request);

                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Failed to update role: User {UserId} not found.", userId);
                return NotFound(new { Error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid role specified for User {UserId}.", userId);
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating role for User {UserId}.", userId);
                return StatusCode(500, new { Error = "An unexpected error occurred." });
            }
        }
        [HttpGet("claims")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllClaimsForReview([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _adminService.GetAllClaimsForReviewAsync(pageNumber, pageSize);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving claims for review.");
                return StatusCode(500, new { Error = "An unexpected error occurred." });
            }
        }
        [HttpGet("policies")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllPolicies([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _adminService.GetAllPoliciesAsync(pageNumber, pageSize);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving policies.");
                return StatusCode(500, new { Error = "An unexpected error occurred." });
            }
        }

    }
}