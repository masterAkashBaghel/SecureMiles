namespace SecureMiles.Common.DTOs.Admin
{
    public class ProposalDetailsResponseDto
    {
        public int ProposalId { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public string? UserPhone { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public DateTime SubmissionDate { get; set; }
        public decimal RequestedCoverage { get; set; }
        public VehicleDto? Vehicle { get; set; }
        public ICollection<DocumentDto>? Documents { get; set; }
    }

    public class VehicleDto
    {
        public int VehicleId { get; set; }
        public string? Make { get; set; }
        public string? Model { get; set; }
        public int Year { get; set; }
        public string? VIN { get; set; }
    }

    public class DocumentDto
    {
        public int DocumentId { get; set; }
        public string? DocumentType { get; set; }
        public string? DocumentUrl { get; set; }
    }
}