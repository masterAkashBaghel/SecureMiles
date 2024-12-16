﻿using Microsoft.AspNetCore.Mvc;
using SecureMiles.Services.Payment;
using SecureMiles.Common.DTOs.Payment;
using System.Threading.Tasks;

namespace SecureMiles.API.Controllers.Payment
{
    [Route("api/paypal")]
    [ApiController]
    public class PayPalController : ControllerBase
    {
        private readonly IPayPalService _payPalService;

        public PayPalController(IPayPalService payPalService)
        {
            _payPalService = payPalService;
        }

        [HttpPost("create-payment")]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var successUrl = $"{Request.Scheme}://{Request.Host}/api/paypal/execute-payment";
            var cancelUrl = $"{Request.Scheme}://{Request.Host}/api/paypal/cancel";

            var response = await _payPalService.CreatePaymentAsync(request.Amount, request.Currency, successUrl, cancelUrl);
            return Ok(response);
        }

        [HttpGet("execute-payment")]
        public async Task<IActionResult> ExecutePayment([FromQuery] string paymentId, [FromQuery] string payerId)
        {
            try
            {
                var response = await _payPalService.ExecutePaymentAsync(paymentId, payerId);
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "An unexpected error occurred." });
            }
        }

    }
}
