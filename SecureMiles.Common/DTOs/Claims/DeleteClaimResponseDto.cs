namespace SecureMiles.Common.DTOs.Claims
{
    public class DeleteClaimResponseDto
    {

        public int ClaimID { get; set; }
        public string? Status { get; set; }

        public string? Message { get; set; }
    }
}