using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SecureMiles.Common.DTOs.Documents;
using SecureMiles.Repositories.Claims;
using SecureMiles.Repositories.Documents;
using SecureMiles.Repositories.Proposals;
using SecureMiles.Services.Cloudinary;

namespace SecureMiles.Services.Document
{
    public class DocumentService : IDocumentService
    {
        private readonly CloudinaryService _cloudinaryService;
        private readonly IDocumentRepository _DocumentRepository;

        private readonly IClaimRepository _ClaimRepository;
        private readonly IProposalsRepository _ProposalRepository;

        public DocumentService(CloudinaryService cloudinaryService, IDocumentRepository IDocumentRepository, IClaimRepository IClaimRepository, IProposalsRepository IProposalRepository)
        {
            _cloudinaryService = cloudinaryService;
            _DocumentRepository = IDocumentRepository;
            _ClaimRepository = IClaimRepository;
            _ProposalRepository = IProposalRepository;
        }


        public async Task<UploadDocumentResponseDto> UploadDocumentAsync(int userId, UploadDocumentRequestDto request)
        {
            // Validate file type and size
            if (request.DocumentFile == null || request.DocumentFile.Length == 0)
            {
                throw new InvalidOperationException("File cannot be empty.");
            }

            // Upload document to Cloudinary
            var uploadResult = await _cloudinaryService.UploadFileAsync(request.DocumentFile, "documents");

            if (uploadResult == null)
            {
                throw new InvalidOperationException("Document upload failed.");
            }

            var proposal = await _ProposalRepository.GetProposalByIdAsync(request.ProposalID, userId);
            if (proposal == null)
            {
                throw new KeyNotFoundException("Proposal not found.");
            }

            // Save document information to the database 
            var document = new Models.Document
            {
                Type = request.Type?.ToString() ?? throw new InvalidOperationException("Document type cannot be null."),
                FilePath = uploadResult.Url.ToString(),
                ClaimID = request.ClaimID,
                ProposalID = request.ProposalID,
                Proposal = proposal ?? throw new InvalidOperationException("Proposal cannot be null."),
                Claim = null,
            };

            // Add document to database  
            var response = await _DocumentRepository.AddAsync(document);

            return response;
        }

        // save a documnet for claim (params claimId, userId, filepath)
        public async Task<Models.Document> SaveDocumentForClaimAsync(int claimId, int userId, IFormFile filePath)
        {
            // get related claim
            var claim = await _ClaimRepository.GetClaimByIdAsync(claimId, userId);
            if (claim == null)
            {
                throw new KeyNotFoundException("Claim not found.");
            }

            // upload document to cloudinary
            var uploadResult = await _cloudinaryService.UploadFileAsync(filePath, "documents");

            if (uploadResult == null)
            {
                throw new InvalidOperationException("Document upload failed.");
            }

            // Save document information to the database
            var document = new Models.Document
            {
                Type = "Claim",
                FilePath = uploadResult.Url.ToString(),
                ClaimID = claimId,
                Claim = claim,
                Proposal = null
            };

            // Add document to database  
            var savedDocument = await _DocumentRepository.AddClaimDocument(document);

            return savedDocument;
        }

        // save a documnet for proposal (params proposalId, userId, filepath)
        public async Task<Models.Document> SaveDocumentForProposalAsync(int userId, int proposalId, IFormFile filePath)
        {
            // get related proposal
            var proposal = await _ProposalRepository.GetProposalByIdAsync(proposalId, userId);
            if (proposal == null)
            {
                throw new KeyNotFoundException("Proposal not found.");
            }

            // upload document to cloudinary
            var uploadResult = await _cloudinaryService.UploadFileAsync(filePath, "documents");

            if (uploadResult == null)
            {
                throw new InvalidOperationException("Document upload failed.");
            }

            // Save document information to the database
            var document = new Models.Document
            {
                Type = "Proposal",
                FilePath = uploadResult.Url.ToString(),
                ProposalID = proposalId,
                Proposal = proposal,
                Claim = null
            };

            // Add document to database  
            var savedDocument = await _DocumentRepository.AddClaimDocument(document);

            return savedDocument;
        }








        public async Task<DocumentDetailsResponseDto> GetDocumentDetailsAsync(int documentId, int userId)
        {
            var document = await _DocumentRepository.GetDocumentByIdAsync(documentId, userId);

            if (document == null)
            {
                throw new KeyNotFoundException("Document not found or unauthorized access.");
            }

            return new DocumentDetailsResponseDto
            {
                DocumentId = document.DocumentID,
                Type = document.Type,
                FilePath = document.FilePath,
                UploadedAt = document.UploadedDate
            };
        }

        public async Task DeleteDocumentAsync(int documentId, int userId)
        {
            // Fetch the document
            var document = await _DocumentRepository.GetDocumentByIdAsync(documentId, userId);
            if (document == null)
            {
                throw new KeyNotFoundException("Document not found or unauthorized access.");
            }

            // Extract the public ID and folder from the URL
            var publicId = Path.GetFileNameWithoutExtension(document.FilePath);
            var folder = "documents";

            Console.WriteLine($"Extracted publicId: {publicId} from FilePath: {document.FilePath}");
            await _cloudinaryService.DeleteFileAsync(publicId, folder);

            // Remove the document metadata from the database
            await _DocumentRepository.DeleteAsync(document);
        }




    }



}