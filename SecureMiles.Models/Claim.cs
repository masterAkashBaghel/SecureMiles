using System;
using System.ComponentModel.DataAnnotations;

namespace SecureMiles.Models
{
    public class Claim
    {
        public int ClaimID { get; set; }

        [Required(ErrorMessage = "Policy ID is required.")]
        public int PolicyID { get; set; }  // Foreign Key to Policy

        [Required(ErrorMessage = "Status is required.")]
        [EnumDataType(typeof(ClaimStatus), ErrorMessage = "Invalid Status.")]
        public string Status { get; set; } = "Pending";  // Default value

        [Required(ErrorMessage = "Incident Date is required.")]
        public DateTime IncidentDate { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public required string Description { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Claim Amount must be greater than 0.")]
        public decimal? ClaimAmount { get; set; }

        public DateTime? ApprovalDate { get; set; }

        [Required(ErrorMessage = "CreatedAt is required.")]
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [Required]
        public required Policy Policy { get; set; }

        [Required]
        public required ICollection<Document> Documents { get; set; }
    }

    public enum ClaimStatus
    {
        Pending,
        Approved,
        Rejected,
        UnderReview,
        Settled
    }
}
