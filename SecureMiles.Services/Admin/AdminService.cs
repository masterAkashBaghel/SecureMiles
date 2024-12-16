
using Microsoft.Extensions.Logging;
using SecureMiles.Common.DTOs.Admin;
using SecureMiles.Repositories.Admin;
using SecureMiles.Repositories.Claims;
using SecureMiles.Services.Mail;

namespace SecureMiles.Services.Admin
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;

        private readonly EmailService _emailService;

        private readonly ILogger<AdminService> _logger;

        private readonly IClaimRepository _claimRepository;


        public AdminService(IAdminRepository adminRepository, ILogger<AdminService> logger, EmailService emailService, IClaimRepository claimRepository)
        {
            _adminRepository = adminRepository;
            _logger = logger;
            _emailService = emailService;
            _claimRepository = claimRepository;

        }


        public async Task<PaginatedUsersResponseDto> GetAllUsersAsync(int pageNumber, int pageSize)
        {
            var (users, totalCount) = await _adminRepository.GetAllUsersAsync(pageNumber, pageSize);

            var userDtos = users.Select(u => new UserResponseDto
            {
                UserId = u.UserID,
                FullName = u.Name,
                Email = u.Email,
                Role = u.Role,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt
            });
            _logger.LogInformation($"Fetched {userDtos.Count()} users from the database.");
            return new PaginatedUsersResponseDto
            {
                Users = userDtos,
                TotalCount = totalCount,
                CurrentPage = pageNumber,
                PageSize = pageSize
            };
        }
        public async Task<UserDetailsResponseDto?> GetUserDetailsAsync(int userId)
        {
            return await _adminRepository.GetUserDetailsByIdAsync(userId);
        }
        public async Task<UpdateUserRoleResponseDto> UpdateUserRoleAsync(int adminId, int userId, UpdateUserRoleRequestDto request)
        {
            // Check if the user exists
            var user = await _adminRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            // Validate the new role
            if (!Enum.TryParse<UserRole>(request.Role, true, out var newRole))
            {
                throw new InvalidOperationException("Invalid role specified.");
            }

            // Update the role
            var oldRole = user.Role;
            await _adminRepository.UpdateUserRoleAsync(user, request.Role);

            // Log role change
            _logger.LogInformation("Admin {AdminId} changed role of User {UserId} from {OldRole} to {NewRole}.", adminId, userId, oldRole, newRole);

            return new UpdateUserRoleResponseDto
            {
                UserId = userId,
                OldRole = oldRole,
                NewRole = request.Role,
                Message = "User role updated successfully."
            };
        }
        public async Task<PaginatedClaimsResponseDto> GetAllClaimsForReviewAsync(int pageNumber, int pageSize)
        {
            var (claims, totalCount) = await _adminRepository.GetAllClaimsForReviewAsync(pageNumber, pageSize);

            return new PaginatedClaimsResponseDto
            {
                Claims = claims,
                TotalCount = totalCount,
                CurrentPage = pageNumber,
                PageSize = pageSize
            };
        }
        public async Task<PaginatedPoliciesResponseDto> GetAllPoliciesAsync(int pageNumber, int pageSize)
        {
            var (policies, totalCount) = await _adminRepository.GetAllPoliciesAsync(pageNumber, pageSize);

            return new PaginatedPoliciesResponseDto
            {
                Policies = policies,
                TotalCount = totalCount,
                CurrentPage = pageNumber,
                PageSize = pageSize
            };
        }


        //  method to get total policy,user,claim and proposals

        public async Task<DashboardDataResponseDto> GetDashboardDataAsync()
        {
            return await _adminRepository.GetDashboardDataAsync();
        }


        // method to get the proposlas along with user details
        public async Task<PaginatedProposalsResponseDto> GetAllProposalsAsync(int pageNumber, int pageSize)
        {
            var response = await _adminRepository.GetAllProposalsAsync(pageNumber, pageSize);

            return new PaginatedProposalsResponseDto
            {
                Proposals = response.Proposals,
                TotalCount = response.TotalCount,
                CurrentPage = pageNumber,
                PageSize = pageSize
            };
        }

        // method to get all details of a proposal

        public async Task<ProposalDetailsResponseDto?> GetProposalDetailsAsync(int proposalId)
        {
            return await _adminRepository.GetProposalDetailsAsync(proposalId);
        }

        // method to delete a proposal 

        public async Task<DeleteProposalResponseDto> DeleteProposalAsync(int proposalId)
        {
            var proposal = await _adminRepository.GetProposalDetailsAsync(proposalId);
            if (proposal == null)
            {
                throw new KeyNotFoundException("Proposal not found.");
            }

            await _adminRepository.DeleteProposalAsync(proposalId);
            _logger.LogInformation("Proposal {ProposalId} deleted successfully.", proposalId);

            return new DeleteProposalResponseDto
            {
                ProposalId = proposalId,
                Message = "Proposal deleted successfully."
            };
        }

        // method to approve  proposal by admin and send email to user
        public async Task<ApproveProposalResponseDto> ApproveProposalAsync(int proposalId)

        {
            var proposal = await _adminRepository.GetProposalDetailsAsync(proposalId);
            if (proposal == null)
            {
                throw new KeyNotFoundException("Proposal not found.");
            }

            await _adminRepository.ApproveProposalAsync(proposalId);
            _logger.LogInformation("Proposal {ProposalId} approved successfully.", proposalId);

            // Send email to user
            var user = await _adminRepository.GetUserByIdAsync(proposal.UserId);
            if (user != null)
            {
                var email = user.Email;
                var subject = "Proposal Approved";
                var body = $"Your proposal with ID {proposalId} has been approved by the admin. You can now proceed with the next steps.";
                await _emailService.SendEmailAsync(email, subject, body);
            }

            return new ApproveProposalResponseDto
            {
                ProposalId = proposalId,
                Message = "Proposal approved successfully."
            };
        }


        // method to reject proposal by admin and send email to user
        public async Task<RejectProposalResponseDto> RejectProposalAsync(int proposalId, RejectProposalRequestDto request)
        {
            var proposal = await _adminRepository.GetProposalDetailsAsync(proposalId);
            if (proposal == null)
            {
                throw new KeyNotFoundException("Proposal not found.");
            }

            await _adminRepository.RejectProposalAsync(proposalId, request.Reason);
            _logger.LogInformation("Proposal {ProposalId} rejected successfully.", proposalId);

            // Send email to user
            var user = await _adminRepository.GetUserByIdAsync(proposal.UserId);
            if (user != null)
            {
                var email = user.Email;
                var subject = "Proposal Rejected";
                var body = $"Your proposal with ID {proposalId} has been rejected by the admin. Reason: {request.Reason}";
                await _emailService.SendEmailAsync(email, subject, body);
            }

            return new RejectProposalResponseDto
            {
                ProposalId = proposalId,
                Message = "Proposal rejected successfully."
            };
        }

        //method to approve claim by admin by taking claim amount as input ,id 
        public async Task<ApproveClaimResponseDto> ApproveClaimAsync(int claimId, ApproveClaimRequestDto request)
        {
            var userID = await _claimRepository.GetClaimByIdAsync(claimId);

            await _adminRepository.ApproveClaimAsync(claimId, request.ClaimAmount);
            _logger.LogInformation("Claim {ClaimId} approved successfully.", claimId);

            // Send email to user
            if (userID.HasValue)
            {
                var user = await _adminRepository.GetUserByIdAsync(userID.Value);
                if (user != null)
                {
                    var email = user.Email;
                    var subject = "Claim Approved";
                    var body = $"Your claim with ID {claimId} has been approved by the admin. You will receive the claim amount soon.";
                    await _emailService.SendEmailAsync(email, subject, body);
                }
            }

            return new ApproveClaimResponseDto
            {
                ClaimId = claimId,
                Message = "Claim approved successfully."
            };
        }
        // method to reject claim by admin by taking claim id as input , without dto
        public async Task<String> RejectClaimAsync(int claimId, string reason)
        {
            var userID = await _claimRepository.GetClaimByIdAsync(claimId);

            await _claimRepository.RejectClaimAsync(claimId, reason);
            _logger.LogInformation("Claim {ClaimId} rejected successfully.", claimId);

            // Send email to user
            if (userID.HasValue)
            {
                var user = await _adminRepository.GetUserByIdAsync(userID.Value);
                if (user != null)
                {
                    var email = user.Email;
                    var subject = "Claim Rejected";
                    var body = $"Your claim with ID {claimId} has been rejected by the admin. Reason: {reason}";
                    await _emailService.SendEmailAsync(email, subject, body);
                }
            }

            return "Claim rejected successfully.";
        }



    }
}