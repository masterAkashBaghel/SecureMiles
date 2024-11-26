
using Microsoft.Extensions.Logging;
using SecureMiles.Common.DTOs.Admin;
using SecureMiles.Repositories.Admin;

namespace SecureMiles.Services.Admin
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;

        private readonly ILogger<AdminService> _logger;


        public AdminService(IAdminRepository adminRepository, ILogger<AdminService> logger)
        {
            _adminRepository = adminRepository;
            _logger = logger;

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

    }
}