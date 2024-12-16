using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecureMiles.Common.DTOs.Admin;
using SecureMiles.Services.Admin;
using SecureMiles.Services.Claims;

namespace SecureMiles.API.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]

    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly ILogger<AdminController> _logger;

        private readonly IClaimService _claimService;


        public AdminController(IAdminService adminService, ILogger<AdminController> logger, IClaimService claimService)
        {
            _adminService = adminService;
            _logger = logger;
            _claimService = claimService;

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

        // method to get total policy,user,claim and proposals
        [HttpGet("dashboard")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDashboardData()
        {
            try
            {
                var response = await _adminService.GetDashboardDataAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving dashboard data.");
                return StatusCode(500, new { Error = "An unexpected error occurred." });
            }

        }


        // method to get all proposals for review
        [HttpGet("proposals")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllProposals([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _adminService.GetAllProposalsAsync(pageNumber, pageSize);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving proposals.");
                return StatusCode(500, new { Error = "An unexpected error occurred." });
            }
        }

        // method to get all details of a proposal

        [HttpGet("proposals/{proposalId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetProposalDetails(int proposalId)
        {
            try
            {
                var response = await _adminService.GetProposalDetailsAsync(proposalId);
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access or proposal not found: {ProposalId}", proposalId);
                return NotFound(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving proposal {ProposalId}.", proposalId);
                return StatusCode(500, new { Error = "An unexpected error occurred." });
            }
        }

        // method to delete a proposal by admin
        [HttpDelete("proposals/{proposalId}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> DeleteProposal(int proposalId)
        {
            try
            {
                var response = await _adminService.DeleteProposalAsync(proposalId);
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access or proposal not found: {ProposalId}", proposalId);
                return NotFound(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting proposal {ProposalId}.", proposalId);
                return StatusCode(500, new { Error = "An unexpected error occurred." });
            }
        }

        // method to approve  proposal by admin

        [HttpPut("proposals/{proposalId}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveProposal(int proposalId)
        {
            try
            {
                var response = await _adminService.ApproveProposalAsync(proposalId);
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access or proposal not found: {ProposalId}", proposalId);
                return NotFound(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while approving proposal {ProposalId}.", proposalId);
                return StatusCode(500, new { Error = "An unexpected error occurred." });
            }
        }

        // method to reject proposal by admin

        [HttpPut("proposals/{proposalId}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RejectProposal(int proposalId, [FromBody] RejectProposalRequestDto request)
        {
            try
            {
                var response = await _adminService.RejectProposalAsync(proposalId, request);
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access or proposal not found: {ProposalId}", proposalId);
                return NotFound(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while rejecting proposal {ProposalId}.", proposalId);
                return StatusCode(500, new { Error = "An unexpected error occurred." });
            }
        }

        //method to approve claim by admin by taking claim amount as input ,id 
        [HttpPut("claims/{claimId}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveClaim(int claimId, [FromBody] ApproveClaimRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var response = await _adminService.ApproveClaimAsync(claimId, request);
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access or claim not found: {ClaimId}", claimId);
                return NotFound(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while approving claim {ClaimId}.", claimId);
                return StatusCode(500, new { Error = "An unexpected error occurred." });
            }
        }

        // method to reject claim by admin by taking claim id as input
        [HttpPut("claims/{claimId}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RejectClaim(int claimId, [FromBody] string notes)
        {
            try
            {
                var response = await _adminService.RejectClaimAsync(claimId, notes);
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access or claim not found: {ClaimId}", claimId);
                return NotFound(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while rejecting claim {ClaimId}.", claimId);
                return StatusCode(500, new { Error = "An unexpected error occurred." });
            }
        }

        // get all the details of a claim by claim id
        [HttpGet("claims/{claimId}/details/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetClaimDetails(int claimId, int userId)
        {
            try
            {
                var response = await _claimService.GetClaimByIdAsync(claimId, userId);
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access or claim not found: {ClaimId}", claimId);
                return NotFound(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching claim details for ClaimID {ClaimId}.", claimId);
                return StatusCode(500, new { Error = "An unexpected error occurred." });
            }
        }

    }
}