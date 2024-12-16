using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace SecureMiles.Models
{
    public class Proposal
    {
        public int ProposalID { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        public int UserID { get; set; }  // Foreign Key to User

        [Required(ErrorMessage = "Vehicle ID is required.")]
        public int VehicleID { get; set; }  // Foreign Key to Vehicle

        public string? PolicyType { get; set; }

        [Required(ErrorMessage = "Requested Coverage is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Requested Coverage must be greater than 0.")]
        public decimal RequestedCoverage { get; set; }

        public decimal PremiumAmount { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [EnumDataType(typeof(ProposalStatus), ErrorMessage = "Invalid Status.")]
        public required string Status { get; set; }

        [Required(ErrorMessage = "Submission Date is required.")]
        public DateTime SubmissionDate { get; set; }

        public DateTime? ApprovalDate { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [Required]
        public required User User { get; set; }

        [Required]
        public required Vehicle Vehicle { get; set; }

        [Required]
        public required ICollection<Document> Documents { get; set; }

        [Required]
        public required Policy Policy { get; set; }
    }

    public enum ProposalStatus
    {
        ProposalSubmitted,
        UnderReview,
        Approved,
        Rejected
    }
}
