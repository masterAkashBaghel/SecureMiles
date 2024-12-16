using System.ComponentModel.DataAnnotations;

namespace SecureMiles.Common.DTOs.Claims

{

    public class ApproveClaimRequestsDto
    {
        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
        public string? Notes { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Approved Amount must be greater than 0.")]
        public decimal ApprovedAmount { get; set; }
    }
}