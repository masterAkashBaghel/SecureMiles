using SecureMiles.Models;
using SecureMiles.Repositories.Claims;
using SecureMiles.Common.DTOs.Claims;
using SecureMiles.Repositories.Policy;
using static SecureMiles.Common.DTOs.Claims.ClaimDetailsDto;
using Microsoft.Extensions.Logging;
using SecureMiles.Services.Document;

namespace SecureMiles.Services.Claims
{
    public class ClaimService : IClaimService
    {
        private readonly IClaimRepository _claimRepository;
        private readonly IPolicyRepository _policyRepository;

        private readonly IDocumentService _documentService;

        private readonly ILogger<ClaimService> _logger;


        public ClaimService(IClaimRepository claimRepository, IPolicyRepository policyRepository, ILogger<ClaimService> logger, IDocumentService documentService)
        {
            _claimRepository = claimRepository;
            _policyRepository = policyRepository;
            _logger = logger;
            _documentService = documentService;
        }

        public async Task<FileClaimResponseDto> FileClaimAsync(int userId, FileClaimRequestDto request)
        {
            // Fetch the policy and validate ownership
            var policy = await _policyRepository.GetPolicyByIdAsync(request.PolicyId, userId);

            if (policy == null || policy.UserID != userId)
            {
                throw new KeyNotFoundException("Policy not found or unauthorized access.");
            }

            if (policy.Status != "Active")
            {
                throw new InvalidOperationException("Claims can only be filed for active policies.");
            }

            // Validate incident date
            if (request.IncidentDate > DateTime.UtcNow)
            {
                throw new InvalidOperationException("Incident date cannot be in the future.");
            }

            // Create the Claim object
            var claim = new Claim
            {
                PolicyID = request.PolicyId,
                IncidentDate = request.IncidentDate,
                Description = request.Description ?? "",
                Status = "Pending", // Initial status
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Documents = new List<Models.Document>(),
                Policy = policy,
                ClaimAmount = string.IsNullOrEmpty(request.ClaimAmount) ? (decimal?)null : decimal.Parse(request.ClaimAmount)
            };

            // Add the claim to the database
            var claimId = await _claimRepository.AddClaimAsync(claim);

            // Save the document if it exists
            if (request.DocumentFile != null)
            {
                var documentResponse = await _documentService.SaveDocumentForClaimAsync(claimId, userId, request.DocumentFile);

                if (documentResponse == null)
                {
                    throw new InvalidOperationException("Document upload failed.");
                }

                claim.Documents.Add(documentResponse);

                // Update the claim in the database with the document reference
                await _claimRepository.UpdateClaimAsync(claim);
            }

            return new FileClaimResponseDto
            {
                ClaimId = claimId,
                Message = "Claim filed successfully.",
                PolicyType = policy.Type,
                CoverageAmount = policy.CoverageAmount,
                PremiumAmount = policy.PremiumAmount
            };
        }
        public async Task<List<ClaimResponseDto>> GetClaimsAsync(int userId)
        {
            var claims = await _claimRepository.GetClaimsByUserIdAsync(userId);

            return claims.Select(c => new ClaimResponseDto
            {
                ClaimId = c.ClaimID,
                PolicyId = c.PolicyID,
                IncidentDate = c.IncidentDate,
                Description = c.Description,
                Status = c.Status,
                ClaimAmount = c.ClaimAmount,
                ApprovalDate = c.ApprovalDate,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                Policy = new PolicyDetailsDto
                {
                    PolicyId = c.Policy.PolicyID,
                    PolicyType = c.Policy.Type,
                    CoverageAmount = c.Policy.CoverageAmount,
                    PremiumAmount = c.Policy.PremiumAmount,
                    PolicyStartDate = c.Policy.PolicyStartDate,
                    PolicyEndDate = c.Policy.PolicyEndDate
                },
                Documents = c.Documents.Select(d => new DocumentResponseDto
                {
                    DocumentId = d.DocumentID,
                    Type = d.Type,
                    FilePath = d.FilePath,
                }).ToList()
            }).ToList();
        }


        public async Task<ClaimDetailsResponseDto> GetClaimByIdAsync(int claimId, int userId)
        {
            var claim = await _claimRepository.GetClaimByIdAsync(claimId, userId);

            if (claim == null)
            {
                throw new KeyNotFoundException("Claim not found or unauthorized access.");
            }

            return new ClaimDetailsResponseDto
            {
                ClaimId = claim.ClaimID,
                PolicyId = claim.PolicyID,
                IncidentDate = claim.IncidentDate,
                Description = claim.Description,
                Status = claim.Status,
                ClaimAmount = claim.ClaimAmount,
                ApprovalDate = claim.ApprovalDate,
                CreatedAt = claim.CreatedAt,
                UpdatedAt = claim.UpdatedAt,
                Policy = claim.Policy != null ? new PolicyDetailsDto
                {
                    PolicyId = claim.Policy.PolicyID,
                    PolicyType = claim.Policy.Type,
                    CoverageAmount = claim.Policy.CoverageAmount,
                    PremiumAmount = claim.Policy.PremiumAmount,
                    PolicyStartDate = claim.Policy.PolicyStartDate,
                } : null
                ,
                Documents = claim.Documents.Select(d => new DocumentDetailsDto
                {
                    DocumentId = d.DocumentID,
                    Type = d.Type,
                    FilePath = d.FilePath
                }).ToList(),

                // also include vehicle details
                Vehicle = claim.Policy?.Vehicle != null ? new ClaimVehicleDto
                {
                    VehicleId = claim.Policy.Vehicle.VehicleID,
                    RegistrationNumber = claim.Policy.Vehicle.RegistrationNumber,
                    Model = claim.Policy.Vehicle.Model,
                    Year = claim.Policy.Vehicle.Year,
                    FuelType = claim.Policy.Vehicle.FuelType,
                    EngineNumber = claim.Policy.Vehicle.EngineNumber,
                    ChassisNumber = claim.Policy.Vehicle.ChassisNumber
                } : null

            };
        }
        public async Task<UpdateClaimResponseDto> UpdateClaimAsync(int claimId, int userId, bool isAdmin, UpdateClaimRequestDto request)
        {
            // Validate request
            if (!isAdmin && request.Status != null)
            {
                throw new UnauthorizedAccessException("Only admins can update claim status.");
            }

            return await _claimRepository.UpdateClaimAsync(claimId, userId, isAdmin, request);
        }
        public async Task<ApproveClaimResponseDto> ApproveClaimAsync(int claimId, ApproveClaimRequestsDto request, bool isAdmin)
        {
            if (!isAdmin)
            {
                throw new UnauthorizedAccessException("Only admins can approve claims.");
            }
            _logger.LogWarning("Approving claim {ClaimId} with amount {ClaimAmount}.", claimId, request);


            return await _claimRepository.ApproveClaimAsync(claimId, request);
        }


        public async Task<RejectClaimResponseDto> RejectClaimAsync(int claimId, RejectClaimRequestDto request, bool isAdmin)
        {
            if (!isAdmin)
            {
                throw new UnauthorizedAccessException("Only admins can reject claims.");
            }

            var notes = request.Notes ?? throw new ArgumentNullException(nameof(request.Notes), "Notes cannot be null.");
            var claim = await _claimRepository.RejectClaimAsync(claimId, notes);

            return new RejectClaimResponseDto
            {
                ClaimId = claim.ClaimID,
                PolicyId = claim.PolicyID,
                Status = claim.Status,
                Notes = claim.Description,
            };
        }

        // method to delete a claim
        public async Task<DeleteClaimResponseDto> DeleteClaimAsync(int claimId, int userId)
        {
            var claim = await _claimRepository.GetClaimByIdAsync(claimId, userId);

            if (claim == null)
            {
                throw new KeyNotFoundException("Claim not found or unauthorized access.");
            }

            var response = await _claimRepository.DeleteClaimAsync(claimId);
            return new DeleteClaimResponseDto
            {
                ClaimID = response.ClaimID,
                Status = response.Status,
                Message = "Claim deleted successfully."
            };
        }

    }
}
