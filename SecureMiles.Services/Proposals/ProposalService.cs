

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SecureMiles.Common.DTOs.Proposals;
using SecureMiles.Common.DTOs.Vehicle;
using SecureMiles.Repositories;
using SecureMiles.Repositories.Proposals;
using SecureMiles.Repositories.Vehicle;
using SecureMiles.Services.Document;

namespace SecureMiles.Services.Proposals
{
    public class ProposalService : IProposalService
    {
        private readonly IProposalsRepository _proposalRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProposalService> _logger;

        private readonly IVehicleRepository _vehicleRepository;

        private readonly IUserRepository _userRepository;

        private readonly IDocumentService _documentService;

        public ProposalService(IProposalsRepository proposalRepository, IConfiguration configuration, ILogger<ProposalService> logger, IVehicleRepository vehicleRepository, IUserRepository userRepository, IDocumentService documentService)
        {
            _proposalRepository = proposalRepository;
            _configuration = configuration;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _vehicleRepository = vehicleRepository;
            _userRepository = userRepository;
            _documentService = documentService;
        }


        public async Task<SubmitProposalResponseDto> SubmitProposalAsync(int userId, SubmitProposalRequestDto request)
        {
            // Validate the vehicle ownership
            var vehicle = await _vehicleRepository.GetVehicleEntityAsync(request.VehicleId, userId);
            var eUser = await _userRepository.GetUserByIdAsync(userId);
            if (vehicle == null || !vehicle.IsActive)
            {
                throw new KeyNotFoundException("Vehicle not found or unauthorized access.");
            }


            var proposal = new Models.Proposal
            {
                VehicleID = request.VehicleId,
                UserID = userId,
                RequestedCoverage = request.RequestedCoverage,
                Status = "Pending", // Initial status
                SubmissionDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Vehicle = vehicle,
                User = eUser,// Set the required User property
                Documents = [], // Initialize the Documents list
                Policy = null,
                PolicyType = request.PolicyType,
                PremiumAmount = (decimal)request.PremiumAmount

            };




            // Add the proposal to the database
            var proposalId = await _proposalRepository.AddProposalAsync(proposal);

            if (request.ProposalDocument != null)
            {
                var response = await _documentService.SaveDocumentForProposalAsync(userId, proposalId, request.ProposalDocument);
                if (response == null)
                {
                    throw new InvalidOperationException("Document upload failed.");
                }
                proposal.Documents.Add(response);

                await _proposalRepository.UpdateProposalAsync(proposal);
            }


            return new SubmitProposalResponseDto
            {
                ProposalId = proposalId,
                Message = "Proposal submitted successfully."
            };
        }


        public async Task<List<AllProposalResponseDto>> GetProposalsAsync(int userId)
        {
            var proposals = await _proposalRepository.GetProposalsByUserIdAsync(userId);

            return proposals.Select(p => new AllProposalResponseDto
            {
                ProposalId = p.ProposalID,
                VehicleId = p.Vehicle.VehicleID,
                VehicleMake = p.Vehicle.Make,
                VehicleModel = p.Vehicle.Model,
                VehicleRegistrationNumber = p.Vehicle.RegistrationNumber,
                RequestedCoverage = p.RequestedCoverage,
                Status = p.Status,
                SubmissionDate = p.SubmissionDate,
                PremiumAmount = p.PremiumAmount,
                PolicyType = p.PolicyType

            }).ToList();
        }
        public async Task<ProposalDetailsResponseDto> GetProposalByIdAsync(int proposalId, int userId)
        {
            var proposal = await _proposalRepository.GetProposalByIdAsync(proposalId, userId);

            if (proposal == null)
            {
                throw new KeyNotFoundException("Proposal not found or unauthorized access.");
            }

            return new ProposalDetailsResponseDto
            {
                ProposalId = proposal.ProposalID,
                RequestedCoverage = proposal.RequestedCoverage,
                Status = proposal.Status,
                SubmissionDate = proposal.SubmissionDate,
                Vehicle = new VehicleResponseDto
                {
                    Make = proposal.Vehicle.Make,
                    Model = proposal.Vehicle.Model,
                    RegistrationNumber = proposal.Vehicle.RegistrationNumber,
                    Type = proposal.Vehicle.Type,

                }
            };
        }
        public async Task<bool> CancelProposalAsync(int proposalId, int userId)
        {
            // Attempt to cancel the proposal in the repository
            var success = await _proposalRepository.CancelProposalAsync(proposalId, userId);

            if (!success)
            {
                throw new KeyNotFoundException("Proposal not found or unauthorized access.");
            }

            return success;
        }

    }
}