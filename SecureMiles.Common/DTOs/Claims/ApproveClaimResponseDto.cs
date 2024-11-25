namespace SecureMiles.Common.DTOs.Claims
{
    public class ApproveClaimResponseDto
    {
        public int ClaimId { get; set; }
        public int PolicyId { get; set; }
        public string? Status { get; set; }
        public decimal ApprovedAmount { get; set; }
        public DateTime ApprovalDate { get; set; }
        public string? Notes { get; set; }
    }

}