

namespace SecureMiles.Common.DTOs.Proposals
{
    public class ProposalResponseDto
    {
        public int ProposalId { get; set; }
        public string? Status { get; set; }
        public DateTime SubmissionDate { get; set; }
        public DateTime? ApprovalDate { get; set; }
    }

}