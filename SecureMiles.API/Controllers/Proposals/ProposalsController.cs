using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecureMiles.Common.DTOs.Proposals;
using SecureMiles.Services.Proposals;

namespace SecureMiles.API.Controllers.Proposals
{
    [ApiController]
    [Route("api/[controller]")]

    public class ProposalsController : ControllerBase
    {
        private readonly IProposalService _proposalService;
        private readonly ILogger<UserController> _logger;


        public ProposalsController(IProposalService proposalService, ILogger<UserController> logger)
        {
            _proposalService = proposalService;
            _logger = logger;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SubmitProposal([FromBody] SubmitProposalRequestDto request)
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
                    return Unauthorized(new { Error = "User ID not found in token." });
                }
                var userId = int.Parse(userIdClaim); // Extract UserID from JWT
                var result = await _proposalService.SubmitProposalAsync(userId, request);
                return Ok(result);

            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access or vehicle not found for Proposal submission.");
                return NotFound(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while submitting a proposal.");
                return StatusCode(500, new { Error = "An unexpected error occurred." });
            }
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetProposals()
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new { Error = "User ID not found or invalid in token." });
                }
                var proposals = await _proposalService.GetProposalsAsync(userId);

                if (!proposals.Any())
                {
                    return NotFound(new { Message = "No proposals found." });
                }

                return Ok(proposals);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching proposals.");
                return StatusCode(500, new { Error = "An unexpected error occurred." });
            }
        }
        [HttpGet("{proposalId}")]
        [Authorize]
        public async Task<IActionResult> GetProposalById(int proposalId)
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new { Error = "User ID not found or invalid in token." });
                }
                var proposalDetails = await _proposalService.GetProposalByIdAsync(proposalId, userId);

                return Ok(proposalDetails);
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
        [HttpDelete("{proposalId}")]
        [Authorize]
        public async Task<IActionResult> CancelProposal(int proposalId)
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new { Error = "User ID not found or invalid in token." });
                }
                await _proposalService.CancelProposalAsync(proposalId, userId);

                return Ok(new { Message = "Proposal canceled successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access or proposal not found: {ProposalId}", proposalId);
                return NotFound(new { Error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Validation error for proposal cancellation: {ProposalId}", proposalId);
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while canceling proposal {ProposalId}.", proposalId);
                return StatusCode(500, new { Error = "An unexpected error occurred." });
            }
        }

    }
}