using Microsoft.EntityFrameworkCore;
using SecureMiles.Common.Data;
using SecureMiles.Common.DTOs.Documents;

namespace SecureMiles.Repositories.Documents
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly InsuranceContext _context;

        public DocumentRepository(InsuranceContext context)
        {
            _context = context;
        }

        // Add the document to the database
        public async Task<UploadDocumentResponseDto> AddAsync(Models.Document document)
        {
            // make it such if it has matching claim id or proposal id then update the document

            // find if there is a document with the same claim id or proposal id
            var existingDocument = await _context.Documents.FirstOrDefaultAsync(d =>
                (d.ClaimID == document.ClaimID || d.ProposalID == document.ProposalID) &&
                d.Type == document.Type);

            if (existingDocument != null)
            {

                // update the existing document
                existingDocument.FilePath = document.FilePath;
                existingDocument.UploadedDate = document.UploadedDate;

                await _context.SaveChangesAsync();

                return new UploadDocumentResponseDto
                {
                    DocumentUrl = existingDocument.FilePath, // The Cloudinary URL
                    DocumentType = existingDocument.Type,    // Document type like IDProof or VehicleRC
                    UploadedAt = existingDocument.UploadedDate // Timestamp of document upload
                };
            }
            else
            {
                // Add the document to the database
                await _context.Documents.AddAsync(document);
                await _context.SaveChangesAsync();

                // Return the response DTO after saving
                return new UploadDocumentResponseDto
                {
                    DocumentUrl = document.FilePath, // The Cloudinary URL
                    DocumentType = document.Type,    // Document type like IDProof or VehicleRC
                    UploadedAt = document.UploadedDate // Timestamp of document upload
                };
            }





            // // Add the document to the database
            // await _context.Documents.AddAsync(document);
            // await _context.SaveChangesAsync();

            // // Return the response DTO after saving
            // return new UploadDocumentResponseDto
            // {
            //     DocumentUrl = document.FilePath, // The Cloudinary URL
            //     DocumentType = document.Type,    // Document type like IDProof or VehicleRC
            //     UploadedAt = document.UploadedDate // Timestamp of document upload
            // };
        }


        public async Task<Models.Document?> GetDocumentByIdAsync(int documentId, int userId)
        {
            return await _context.Documents
                .Include(d => d.Claim)
                .ThenInclude(c => c.Policy) // Navigate to User through Policy if needed
                .Include(d => d.Proposal)
                .ThenInclude(p => p.User) // Navigate to User through Proposal
                .FirstOrDefaultAsync(d =>
                    d.DocumentID == documentId &&
                    (
                        (d.Claim != null && d.Claim.Policy.UserID == userId) || // Document associated with a Claim
                        (d.Proposal != null && d.Proposal.UserID == userId)    // Document associated with a Proposal
                    ));
        }




        public async Task DeleteAsync(Models.Document document)
        {
            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();
        }



    }
}