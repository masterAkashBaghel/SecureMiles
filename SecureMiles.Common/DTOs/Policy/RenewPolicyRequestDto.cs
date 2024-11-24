using System.ComponentModel.DataAnnotations;

namespace SecureMiles.Common.DTOs.Policy

{
    public class RenewPolicyRequestDto
    {
        [Required(ErrorMessage = "Renewal period is required.")]
        [Range(1, 24, ErrorMessage = "Renewal period must be between 1 and 24 months.")]
        public int RenewalPeriodMonths { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Premium Amount must be greater than 0.")]
        public decimal? PremiumAmount { get; set; } // Optional: Admin can override
    }
}
