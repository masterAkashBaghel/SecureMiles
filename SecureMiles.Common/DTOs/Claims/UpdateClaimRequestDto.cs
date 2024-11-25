using System.ComponentModel.DataAnnotations;

namespace SecureMiles.Common.DTOs.Claims
{

    public class UpdateClaimRequestDto
    {
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string? Description { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Claim Amount must be greater than 0.")]
        public decimal? ClaimAmount { get; set; }

        [EnumDataType(typeof(ClaimStatus), ErrorMessage = "Invalid Status.")]
        public string? Status { get; set; } // Admin-only field
    }

    public enum ClaimStatus
    {
        Pending,
        Approved,
        Rejected,
        UnderReview,
        Settled
    }

}