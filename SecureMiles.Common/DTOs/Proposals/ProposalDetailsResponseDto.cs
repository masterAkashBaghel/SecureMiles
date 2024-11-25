
using SecureMiles.Common.DTOs.Vehicle;

namespace SecureMiles.Common.DTOs.Proposals
{
    public class ProposalDetailsResponseDto
    {
        public int ProposalId { get; set; }
        public decimal RequestedCoverage { get; set; }
        public string? Status { get; set; } // Pending, Approved, Rejected
        public DateTime SubmissionDate { get; set; }

        // Associated vehicle details
        public VehicleResponseDto? Vehicle { get; set; }
    }
}