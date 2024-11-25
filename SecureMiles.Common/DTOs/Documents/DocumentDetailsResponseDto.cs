namespace SecureMiles.Common.DTOs.Documents
{
    public class DocumentDetailsResponseDto
    {
        public int DocumentId { get; set; }
        public string? Type { get; set; }
        public string? FilePath { get; set; }
        public DateTime UploadedAt { get; set; }
    }

}