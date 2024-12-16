

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecureMiles.Services.FakePayment;

namespace SecureMiles.API.Controllers.Payment
{
    [ApiController]
    [Route("api/payments")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        [HttpPost("{proposalId}")]
        [Authorize]
        public async Task<IActionResult> MakePayment(int proposalId)
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out var userId))
                {
                    _logger.LogWarning("Invalid user ID claim: {UserIdClaim}", userIdClaim);
                    return Unauthorized(new { Error = "Invalid user ID." });
                }

                var response = await _paymentService.ProcessPaymentAsync(userId, proposalId);

                return Ok(new
                {
                    Message = "Payment successful, policy created.",
                    PolicyDetails = response.PolicyDetails
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing payment for proposal: {ProposalId}", proposalId);
                return StatusCode(500, new { Error = "An unexpected error occurred." });
            }
        }
    }

}