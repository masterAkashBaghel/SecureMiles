using System;
using System.ComponentModel.DataAnnotations;

namespace SecureMiles.Models
{
    public class Document
    {
        public int DocumentID { get; set; }

        public int? ProposalID { get; set; }  // Foreign Key to Proposal, nullable
        public int? ClaimID { get; set; }  // Foreign Key to Claim, nullable

        [Required(ErrorMessage = "Document Type is required.")]
        [StringLength(50, ErrorMessage = "Document Type cannot exceed 50 characters.")]
        public required string Type { get; set; }

        [Required(ErrorMessage = "File Path is required.")]
        [StringLength(255, ErrorMessage = "File Path cannot exceed 255 characters.")]
        public required string FilePath { get; set; }

        [Required(ErrorMessage = "Upload Date is required.")]

        public DateTime UploadedDate { get; set; }

        // Navigation properties
        public required Proposal Proposal { get; set; }
        public required Claim Claim { get; set; }
    }
}
