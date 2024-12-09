using Microsoft.Extensions.Configuration;
using PayPal.Api;
using SecureMiles.Models;
using SecureMiles.Repositories.Payment;
using SecureMiles.Common.DTOs.Payment;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace SecureMiles.Services.Payment
{
    public class PayPalService : IPayPalService
    {
        private readonly IConfiguration _configuration;
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILogger<PayPalService> _logger;

        public PayPalService(IConfiguration configuration, IPaymentRepository paymentRepository, ILogger<PayPalService> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<CreatePaymentResponseDto> CreatePaymentAsync(decimal amount, string currency, string successUrl, string cancelUrl)
        {
            var apiContext = GetApiContext();

            var payment = new PayPal.Api.Payment
            {
                intent = "sale",
                payer = new Payer { payment_method = "paypal" },
                transactions = new List<Transaction>
                {
                    new() {
                        amount = new Amount
                        {
                            currency = currency,
                            total = amount.ToString("F2")
                        },
                        description = "SecureMiles Payment"
                    }
                },
                redirect_urls = new RedirectUrls
                {
                    return_url = successUrl,
                    cancel_url = cancelUrl
                }
            };

            var createdPayment = await Task.Run(() => payment.Create(apiContext));
            var approvalUrl = createdPayment.links.FirstOrDefault(l => l.rel == "approval_url")?.href;

            return new CreatePaymentResponseDto
            {
                ApprovalUrl = approvalUrl,
                Message = "Payment created successfully."
            };
        }

        public async Task<ExecutePaymentResponseDto> ExecutePaymentAsync(string paymentId, string payerId)
        {
            var apiContext = GetApiContext();
            var paymentExecution = new PaymentExecution { payer_id = payerId };
            var payment = new PayPal.Api.Payment { id = paymentId };

            var executedPayment = await Task.Run(() => payment.Execute(apiContext, paymentExecution));

            return new ExecutePaymentResponseDto
            {
                Status = executedPayment.state,
                TransactionId = executedPayment.id,
                Message = "Payment executed successfully."
            };
        }

        public async Task<int> SavePaymentAsync(string transactionId, decimal amount, string currency, string status, int userId, int policyId)
        {
            var payment = new SecureMiles.Models.Payment
            {
                TransactionId = transactionId,
                Amount = amount,
                Currency = currency,
                Status = status,
                UserId = userId,
                PolicyId = policyId,
                CreatedAt = DateTime.UtcNow
            };

            return await _paymentRepository.AddPaymentAsync(payment);
        }

        private APIContext GetApiContext()
        {
            var clientId = _configuration["PayPal:ClientId"];
            var clientSecret = _configuration["PayPal:ClientSecret"];

            var config = new Dictionary<string, string>
            {
                { "mode", _configuration["PayPal:Mode"] },
                { "clientId", clientId },
                { "clientSecret", clientSecret }
            };

            var accessToken = new OAuthTokenCredential(clientId, clientSecret, config).GetAccessToken();
            return new APIContext(accessToken);
        }
    }
}
