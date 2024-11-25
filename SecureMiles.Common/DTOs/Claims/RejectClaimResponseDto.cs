

namespace SecureMiles.Common.DTOs.Claims
{
    public class RejectClaimResponseDto
    {
        public int ClaimId { get; set; }
        public int PolicyId { get; set; }
        public string? Status { get; set; }
        public string? Notes { get; set; }
        public DateTime RejectionDate { get; set; }
    }

}