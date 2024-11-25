
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecureMiles.Common.DTOs.Claims;
using SecureMiles.Services.Claims;

namespace SecureMiles.API.Controllers.Claims
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClaimsController : ControllerBase
    {
        private readonly IClaimService _claimService;
        private readonly ILogger<ClaimsController> _logger;

        public ClaimsController(IClaimService claimService, ILogger<ClaimsController> logger)
        {
            _claimService = claimService;
            _logger = logger;
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> FileClaim([FromBody] FileClaimRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null)
                {
                    throw new UnauthorizedAccessException("User ID claim not found.");
                }
                var userId = int.Parse(userIdClaim); // Extract UserID from JWT
                var result = await _claimService.FileClaimAsync(userId, request);

                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access or policy not found for Claim submission.");
                return NotFound(new { Error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Validation error for Claim submission.");
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while filing a claim.");
                return StatusCode(500, new { Error = "An unexpected error occurred." });
            }
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetClaims()
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null)
                {
                    throw new UnauthorizedAccessException("User ID claim not found.");
                }
                var userId = int.Parse(userIdClaim); // Extract UserID from JWT
                var claims = await _claimService.GetClaimsAsync(userId);

                if (!claims.Any())
                {
                    return NotFound(new { Message = "No claims found." });
                }

                return Ok(claims);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching claims.");
                return StatusCode(500, new { Error = "An unexpected error occurred." });
            }
        }


        [HttpGet("{claimId}")]
        [Authorize]
        public async Task<IActionResult> GetClaimById(int claimId)
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null)
                {
                    throw new UnauthorizedAccessException("User ID claim not found.");
                }
                var userId = int.Parse(userIdClaim); // Extract UserID from JWT
                var claimDetails = await _claimService.GetClaimByIdAsync(claimId, userId);

                return Ok(claimDetails);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access or claim not found: {ClaimId}", claimId);
                return NotFound(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving claim {ClaimId}.", claimId);
                return StatusCode(500, new { Error = "An unexpected error occurred." });
            }
        }

        [HttpPut("{claimId}")]
        [Authorize]
        public async Task<IActionResult> UpdateClaim(int claimId, [FromBody] UpdateClaimRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
                {
                    throw new UnauthorizedAccessException("User ID claim not found or invalid.");
                }
                var isAdmin = User.IsInRole("Admin"); // Check if the user is an admin

                var result = await _claimService.UpdateClaimAsync(claimId, userId, isAdmin, request);

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized status update attempt for claim {ClaimId}.", claimId);
                return Forbid();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Claim not found: {ClaimId}", claimId);
                return NotFound(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating claim {ClaimId}.", claimId);
                return StatusCode(500, new { Error = "An unexpected error occurred." });
            }
        }
        [HttpPost("{claimId}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveClaim(int claimId, [FromBody] ApproveClaimRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {

                var result = await _claimService.ApproveClaimAsync(claimId, request, true);

                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Claim not found or not eligible for approval: {ClaimId}", claimId);
                return NotFound(new { Error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access attempt for claim approval: {ClaimId}", claimId);
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while approving claim {ClaimId}.", claimId);
                return StatusCode(500, new { Error = "An unexpected error occurred." });
            }
        }
        [HttpPost("{claimId}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RejectClaim(int claimId, [FromBody] RejectClaimRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null)
                {
                    throw new UnauthorizedAccessException("User ID claim not found.");
                }
                var userId = int.Parse(userIdClaim); // Extract UserID from JWT
                var isAdmin = User.IsInRole("Admin"); // Check if the user is an admin

                var result = await _claimService.RejectClaimAsync(claimId, request, isAdmin);

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized attempt to reject claim {ClaimId}.", claimId);
                return Forbid();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Claim not found: {ClaimId}", claimId);
                return NotFound(new { Error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Claim cannot be rejected: {ClaimId}", claimId);
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while rejecting claim {ClaimId}.", claimId);
                return StatusCode(500, new { Error = "An unexpected error occurred." });
            }
        }

    }
}