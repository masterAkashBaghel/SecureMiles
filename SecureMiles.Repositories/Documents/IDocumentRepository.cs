using SecureMiles.Common.DTOs.Documents;

namespace SecureMiles.Repositories.Documents
{



    public interface IDocumentRepository
    {
        Task<UploadDocumentResponseDto> AddAsync(Models.Document document);
        Task<Models.Document?> GetDocumentByIdAsync(int documentId, int userId);
        Task DeleteAsync(Models.Document document);
    }


}