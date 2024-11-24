

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecureMiles.Common.DTOs.Policy;
using SecureMiles.Services.Policy;

namespace SecureMiles.API.Controllers.Policy
{
    [ApiController]
    [Route("api/[controller]")]
    public class PolicyController : ControllerBase
    {
        private readonly IPolicyServices _policyService;
        private readonly ILogger<PolicyController> _logger;

        public PolicyController(IPolicyServices policyService, ILogger<PolicyController> logger)
        {
            _policyService = policyService;
            _logger = logger;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePolicy([FromBody] CreatePolicyRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid policy creation request: {Request}", request);
                return BadRequest(ModelState);
            }

            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null)
                {
                    return Unauthorized(new { Error = "User identifier claim is missing." });
                }
                var userId = int.Parse(userIdClaim);
                var result = await _policyService.CreatePolicyAsync(userId, request);

                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access or vehicle not found for Policy creation.");
                return NotFound(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a policy.");
                return StatusCode(500, new { Error = "An unexpected error occurred." });
            }
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPolicies()
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null)
                {
                    return Unauthorized(new { Error = "User identifier claim is missing." });
                }
                var userId = int.Parse(userIdClaim); // Extract UserID from JWT
                var policies = await _policyService.GetPoliciesAsync(userId);

                if (!policies.Any())
                {
                    return NotFound(new { Message = "No policies found." });
                }

                return Ok(policies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving policies for user.");
                return StatusCode(500, new { Error = "An unexpected error occurred." });
            }
        }
        [HttpGet("{policyId}")]
        [Authorize]
        public async Task<IActionResult> GetPolicyById(int policyId)
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null)
                {
                    _logger.LogWarning("User identifier claim is missing.");
                    return Unauthorized(new { Error = "User identifier claim is missing." });
                }
                var userId = int.Parse(userIdClaim); // Extract UserID from JWT
                var policyDetails = await _policyService.GetPolicyByIdAsync(policyId, userId);

                return Ok(policyDetails);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access or policy not found: {PolicyId}", policyId);
                return NotFound(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving policy {PolicyId}", policyId);
                return StatusCode(500, new { Error = "An unexpected error occurred." });
            }
        }



        [HttpPut("{policyId}")]
        [Authorize]
        public async Task<IActionResult> UpdatePolicy(int policyId, [FromBody] UpdatePolicyRequestDto request)
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
                    return Unauthorized(new { Error = "User identifier claim is missing." });
                }
                var userId = int.Parse(userIdClaim); // Extract UserID from JWT
                var isAdmin = User.IsInRole("Admin"); // Check if the user is an admin
                _logger.LogInformation("User {UserId} is an admin: {IsAdmin}", userId, isAdmin);

                var result = await _policyService.UpdatePolicyAsync(policyId, userId, request, isAdmin);

                return Ok(new { Message = "Policy updated successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access or policy not found: {PolicyId}", policyId);
                return NotFound(new { Error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Validation error for policy update: {PolicyId}", policyId);
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating policy {PolicyId}", policyId);
                return StatusCode(500, new { Error = "An unexpected error occurred." });
            }
        }


        [HttpPost("{policyId}/renew")]
        [Authorize]
        public async Task<IActionResult> RenewPolicy(int policyId, [FromBody] RenewPolicyRequestDto request)
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
                    return Unauthorized(new { Error = "User identifier claim is missing." });
                }
                var userId = int.Parse(userIdClaim); // Extract UserID from JWT
                var result = await _policyService.RenewPolicyAsync(policyId, userId, request);

                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access or policy not found: {PolicyId}", policyId);
                return NotFound(new { Error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Validation error for policy renewal: {PolicyId}", policyId);
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while renewing policy {PolicyId}", policyId);
                return StatusCode(500, new { Error = "An unexpected error occurred." });
            }
        }

    }

}