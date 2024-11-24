
using System.ComponentModel.DataAnnotations;

namespace SecureMiles.Common.DTOs.Policy
{

    public class CreatePolicyRequestDto
    {
        [Required(ErrorMessage = "Vehicle ID is required.")]
        public int VehicleId { get; set; }

        [Required(ErrorMessage = "Policy Type is required.")]
        [EnumDataType(typeof(PolicyType), ErrorMessage = "Invalid Policy Type.")]
        public string? PolicyType { get; set; } // e.g., Comprehensive, Third-Party

        [Required(ErrorMessage = "Coverage Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Coverage Amount must be greater than 0.")]
        public decimal CoverageAmount { get; set; }

        [Required(ErrorMessage = "Premium Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Premium Amount must be greater than 0.")]
        public decimal PremiumAmount { get; set; }

        [Required(ErrorMessage = "Policy Start Date is required.")]
        public DateTime PolicyStartDate { get; set; }

        [Required(ErrorMessage = "Policy End Date is required.")]
        public DateTime PolicyEndDate { get; set; }
    }

    public enum PolicyType
    {
        Comprehensive,
        ThirdParty,
        FireAndTheft
    }

}