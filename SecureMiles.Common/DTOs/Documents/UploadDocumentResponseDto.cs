namespace SecureMiles.Common.DTOs.Documents
{
    public class UploadDocumentResponseDto
    {
        public string? DocumentUrl { get; set; }
        public string? DocumentType { get; set; }
        public DateTime UploadedAt { get; set; }
    }

}