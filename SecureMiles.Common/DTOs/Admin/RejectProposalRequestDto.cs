

namespace SecureMiles.Common.DTOs.Admin
{
    public class RejectProposalRequestDto
    {
        public int ProposalId { get; set; }
        public string? Reason { get; set; }
    }
}