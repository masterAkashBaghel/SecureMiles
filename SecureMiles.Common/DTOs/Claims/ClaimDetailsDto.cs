namespace SecureMiles.Common.DTOs.Claims
{
    public class ClaimDetailsDto
    {
        public class ClaimResponseDto
        {
            public int ClaimId { get; set; }
            public int PolicyId { get; set; }
            public DateTime IncidentDate { get; set; }
            public string? Description { get; set; }
            public string? Status { get; set; } // Pending, Approved, etc.
            public decimal? ClaimAmount { get; set; }
            public DateTime? ApprovalDate { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }

            // Associated policy details
            public PolicyDetailsDto? Policy { get; set; }

            // Associated document details
            public List<DocumentResponseDto>? Documents { get; set; }
        }

        public class DocumentResponseDto
        {
            public int DocumentId { get; set; }
            public string? Type { get; set; } // e.g., Accident Report, RC, etc.
            public string? FilePath { get; set; } // File location
            public DateTime UploadedAt { get; set; }
        }

    }
    public class PolicyDetailsDto
    {
        public int PolicyId { get; set; }
        public string? PolicyType { get; set; } // Comprehensive, Third-Party, etc.
        public decimal CoverageAmount { get; set; }
        public decimal PremiumAmount { get; set; }
        public DateTime PolicyStartDate { get; set; }
        public DateTime PolicyEndDate { get; set; }
    }
}