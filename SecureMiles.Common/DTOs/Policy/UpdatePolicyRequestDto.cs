using System.ComponentModel.DataAnnotations;

namespace SecureMiles.Common.DTOs.Policy
{

    public class UpdatePolicyRequestDto
    {
        [Required(ErrorMessage = "Policy ID is required.")]
        public int PolicyId { get; set; }

        [StringLength(50, ErrorMessage = "Policy Type cannot exceed 50 characters.")]
        public string? PolicyType { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Coverage Amount must be greater than 0.")]
        public decimal? CoverageAmount { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Premium Amount must be greater than 0.")]
        public decimal? PremiumAmount { get; set; }

        public DateTime? PolicyStartDate { get; set; }
        public DateTime? PolicyEndDate { get; set; }

        [StringLength(20, ErrorMessage = "Status cannot exceed 20 characters.")]
        public string? Status { get; set; } // Active, Expired, Terminated
    }

}