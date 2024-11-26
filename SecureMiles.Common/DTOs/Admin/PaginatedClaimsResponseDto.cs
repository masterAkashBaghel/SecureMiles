namespace SecureMiles.Common.DTOs.Admin
{
    public class PaginatedClaimsResponseDto
    {
        public IEnumerable<AdminClaimResponseDto>? Claims { get; set; }
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }

}