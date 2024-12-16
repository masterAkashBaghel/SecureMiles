

namespace SecureMiles.Common.DTOs.Admin
{
    public class PaginatedProposalsResponseDto
    {
        public IEnumerable<ProposalResponseDto>? Proposals { get; set; }
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }

    public class ProposalResponseDto
    {
        public int ProposalId { get; set; }
        public string? PolicyName { get; set; }
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public string? UserPhone { get; set; }
        public string? Status { get; set; }

        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}