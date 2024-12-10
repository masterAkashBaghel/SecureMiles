namespace SecureMiles.Common.DTOs.Proposals
{
    public class AllProposalResponseDto
    {
        public int ProposalId { get; set; }
        public int VehicleId { get; set; }
        public string? VehicleMake { get; set; }
        public string? VehicleModel { get; set; }
        public string? VehicleRegistrationNumber { get; set; }
        public decimal RequestedCoverage { get; set; }
        public string? Status { get; set; } // Pending, Approved, Rejected
        public DateTime SubmissionDate { get; set; }

        public decimal? PremiumAmount { get; set; }

        public string? PolicyType { get; set; }
    }


}