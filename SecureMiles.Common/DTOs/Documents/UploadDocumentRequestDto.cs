using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace SecureMiles.Common.DTOs.Documents
{
    public class UploadDocumentRequestDto
    {
        [Required(ErrorMessage = "Document type is required.")]
        public string? Type { get; set; }

        [Required(ErrorMessage = "Document file is required.")]
        public IFormFile? DocumentFile { get; set; }

        public int? ClaimID { get; set; }  // Optional: can be null if the document is related to a proposal
        public int? ProposalID { get; set; }  // Optional: can be null if the document is related to a claim

    }





}