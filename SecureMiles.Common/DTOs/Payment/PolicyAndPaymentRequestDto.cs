using System.ComponentModel.DataAnnotations;

namespace SecureMiles.Common.DTOs.Payment
{
    public class PolicyAndPaymentRequestDto
    {
        [Required(ErrorMessage = "Proposal ID is required.")]
        public int ProposalId { get; set; }

        [Required(ErrorMessage = "Payment method is required.")]
        public string? PaymentMethod { get; set; } // E.g., "Credit Card", "UPI", "Net Banking"

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        public decimal Amount { get; set; } // Policy premium amount

        [Required(ErrorMessage = "Currency is required.")]
        public string Currency { get; set; } = "INR"; // Default to INR
    }
}
