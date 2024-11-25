

namespace SecureMiles.Common.DTOs.Claims
{
    public class UpdateClaimResponseDto
    {
        public int ClaimId { get; set; }
        public int PolicyId { get; set; }
        public string? Status { get; set; }
        public string? Description { get; set; }
        public decimal? ClaimAmount { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

}