using Microsoft.Extensions.Logging;
using SecureMiles.Repositories.Policy;
using SecureMiles.Services.Mail;
using SecureMiles.Repositories.Proposals;
using SecureMiles.Common.DTOs.Payment;
using SecureMiles.Repositories.FakePayment;
using QuestPDF.Fluent;
using QuestPDF.Previewer;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SecureMiles.Models;
using SecureMiles.Repositories;



namespace SecureMiles.Services.FakePayment



{
    public class PaymentService : IPaymentService
    {
        private readonly IPolicyRepository _policyRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IProposalsRepository _proposalRepository;

        private readonly IUserRepository _userRepository;
        private readonly EmailService _emailService;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(
            IPolicyRepository policyRepository,
            IPaymentRepository paymentRepository,
            IProposalsRepository proposalRepository,
            IUserRepository userRepository,
            EmailService emailService,
            ILogger<PaymentService> logger)
        {
            _policyRepository = policyRepository;
            _paymentRepository = paymentRepository;
            _proposalRepository = proposalRepository;
            _userRepository = userRepository;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<PaymentResponseDto> ProcessPaymentAsync(int userId, int proposalId)
        {
            // Fetch the proposal
            var proposal = await _proposalRepository.GetProposalByIdAsync(proposalId);
            if (proposal == null || proposal.UserID != userId)
            {
                throw new KeyNotFoundException("Proposal not found or unauthorized access.");
            }

            if (proposal.Status != "Approved")
            {
                throw new InvalidOperationException("Only approved proposals can proceed with payment.");
            }

            // Create the Policy
            var policy = new Models.Policy
            {
                UserID = proposal.UserID,
                VehicleID = proposal.VehicleID,
                Type = proposal.PolicyType ?? "DefaultPolicyType",
                PremiumAmount = proposal.PremiumAmount,
                PolicyStartDate = DateTime.UtcNow,
                PolicyEndDate = DateTime.UtcNow.AddYears(1),
                RenewalReminderDate = DateTime.UtcNow.AddYears(1).AddDays(-30),
                Status = "Active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Claims = new List<Models.Claim>(),
                Payments = new List<Models.Payment>(),
                User = proposal.User,
                Vehicle = proposal.Vehicle,
                Proposal = proposal
            };

            var policyId = await _policyRepository.AddPolicyAsync(policy);

            // Add the Payment
            var payment = new Models.Payment
            {
                PolicyId = policyId,
                UserId = userId,
                Amount = proposal.PremiumAmount,
                Status = "Completed",
                TransactionId = Guid.NewGuid().ToString(), // Simulated transaction ID
                CreatedAt = DateTime.UtcNow,
                Currency = "INR"
            };
            var eUser = await _userRepository.GetUserByIdAsync(userId);
            await _paymentRepository.AddPaymentAsync(payment);

            // Update Proposal
            proposal.Status = "Converted to Policy";
            await _proposalRepository.UpdateProposalAsync(proposal);

            // Generate Policy Document (PDF)
            var policyDocument = GeneratePolicyDocument(policy);


            // Notify User
            var emailContent = $"Dear User,\n\nYour policy (ID: {policyId}) has been created successfully.\n\nThank you for choosing SecureMiles.";
            // proper check and log all data before sending email
            if (eUser == null)
            {
                _logger.LogError("User not found for proposal: {ProposalId}", proposalId);
                throw new KeyNotFoundException("User not found for proposal.");
            }
            if (string.IsNullOrEmpty(proposal.User.Email))
            {
                _logger.LogError("Email not found for user: {UserId}", userId);
                throw new KeyNotFoundException("Email not found for user.");
            }
            if (policyDocument == null)
            {
                _logger.LogError("Policy document could not be generated for policy: {PolicyId}", policyId);
                throw new InvalidOperationException("Policy document could not be generated.");
            }
            await _emailService.SendEmailWithAttachmentAsync(eUser.Email, "Policy Created", emailContent, policyDocument, "PolicyDocument.pdf");
            _logger.LogInformation("Payment processed and policy created for proposal: {ProposalId}", proposalId);

            return new PaymentResponseDto
            {
                PolicyDetails = new PolicyResponseDto
                {
                    PolicyId = policyId,
                    PolicyType = policy.Type,
                    CoverageAmount = policy.CoverageAmount,
                    PremiumAmount = policy.PremiumAmount,
                    PolicyStartDate = policy.PolicyStartDate,
                    PolicyEndDate = policy.PolicyEndDate,
                    Status = policy.Status
                }
            };
        }



        private byte[] GeneratePolicyDocument(Models.Policy policy)
        {
            // Set the appropriate license type
            QuestPDF.Settings.License = LicenseType.Community;

            var document = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(20));

                    page.Header()
                        .Text("SecureMiles Policy Document")
                        .SemiBold().FontSize(36).FontColor(Colors.Blue.Medium);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(column =>
                        {
                            column.Item().Text($"Policy ID: {policy.PolicyID}");
                            column.Item().Text($"Type: {policy.Type}");
                            column.Item().Text($"Coverage: {policy.CoverageAmount}");
                            column.Item().Text($"Premium: {policy.PremiumAmount}");
                            column.Item().Text($"Start Date: {policy.PolicyStartDate:yyyy-MM-dd}");
                            column.Item().Text($"End Date: {policy.PolicyEndDate:yyyy-MM-dd}");
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Page ");
                            x.CurrentPageNumber();
                        });
                });
            });

            return document.GeneratePdf();
        }
    }

}