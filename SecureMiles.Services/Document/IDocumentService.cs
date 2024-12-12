using Microsoft.AspNetCore.Http;
using SecureMiles.Common.DTOs.Documents;

namespace SecureMiles.Services.Document
{
    public interface IDocumentService
    {
        Task<UploadDocumentResponseDto> UploadDocumentAsync(int userId, UploadDocumentRequestDto request);
        Task<DocumentDetailsResponseDto> GetDocumentDetailsAsync(int documentId, int userId);

        Task DeleteDocumentAsync(int documentId, int userId);

        Task<Models.Document> SaveDocumentForClaimAsync(int claimId, int userId, IFormFile filePath);

        Task<Models.Document> SaveDocumentForProposalAsync(int proposalId, int userId, IFormFile filePath);


    }
}