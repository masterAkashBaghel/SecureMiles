namespace SecureMiles.Common.DTOs.Admin
{
    public class PaginatedPoliciesResponseDto
    {
        public IEnumerable<AdminPolicyResponseDto>? Policies { get; set; }
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }

}