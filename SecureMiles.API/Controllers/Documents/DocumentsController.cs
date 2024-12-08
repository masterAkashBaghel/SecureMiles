
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecureMiles.Common.DTOs.Documents;
using SecureMiles.Services.Document;

namespace SecureMiles.API.Controllers.Documents
{
    [ApiController]
    [Route("api/[controller]")]


    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentService _documentService;
        private readonly ILogger<DocumentsController> _logger;

        public DocumentsController(IDocumentService documentService, ILogger<DocumentsController> logger)
        {
            _documentService = documentService;
            _logger = logger;
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UploadDocument([FromForm] UploadDocumentRequestDto request)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null)
                {
                    return Unauthorized(new { Error = "User ID not found in token." });
                }
                var userId = int.Parse(userIdClaim); // Extract UserID from JWT
                var result = await _documentService.UploadDocumentAsync(userId, request);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access or user not found for document upload.");

                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while uploading a document.");
                return StatusCode(500, new { Error = "An unexpected error occurred." });
            }
        }
        [HttpGet("{documentId}")]
        [Authorize]
        public async Task<IActionResult> GetDocument(int documentId)
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new { Error = "Invalid user ID in token." });
                }
                var documentDetails = await _documentService.GetDocumentDetailsAsync(documentId, userId);

                return Ok(documentDetails);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Document not found or unauthorized access: {DocumentId}", documentId);
                return NotFound(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving document {DocumentId}.", documentId);
                return StatusCode(500, new { Error = "An unexpected error occurred." });
            }
        }


        [HttpDelete("{documentId}")]
        [Authorize]
        public async Task<IActionResult> DeleteDocument(int documentId)
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null)
                {
                    return Unauthorized(new { Error = "User ID not found in token." });
                }
                var userId = int.Parse(userIdClaim); // Extract UserID from JWT
                await _documentService.DeleteDocumentAsync(documentId, userId);

                var response = new { Message = "Document deleted successfully." };
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Document not found or unauthorized access: {DocumentId}", documentId);
                return NotFound(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting document {DocumentId}.", documentId);
                return StatusCode(500, new { Error = "An unexpected error occurred." });
            }
        }

    }
}