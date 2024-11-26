using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SecureMiles.Common.Data;
using SecureMiles.Common.DTOs.Admin;
using SecureMiles.Common.DTOs.Policy;
using SecureMiles.Common.DTOs.Vehicle;
using SecureMiles.Models;
using static SecureMiles.Common.DTOs.Claims.ClaimDetailsDto;

namespace SecureMiles.Repositories.Admin
{
    public class AdminRepository : IAdminRepository
    {
        private readonly InsuranceContext _context;

        public AdminRepository(InsuranceContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<User>, int)> GetAllUsersAsync(int pageNumber, int pageSize)
        {
            var query = _context.Users.OrderBy(u => u.CreatedAt);

            var totalCount = await query.CountAsync();
            var users = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (users, totalCount);
        }
        public async Task<UserDetailsResponseDto?> GetUserDetailsByIdAsync(int userId)
        {
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "GetUserDetails";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@UserId", userId));

            await _context.Database.OpenConnectionAsync();

            using var reader = await command.ExecuteReaderAsync();
            var userDetails = new UserDetailsResponseDto();

            // Fetch user details
            if (await reader.ReadAsync())
            {
                userDetails.UserId = reader.GetInt32("UserID");
                userDetails.Name = reader.GetString("Name");
                userDetails.Address = reader.GetString("Address");
                userDetails.City = reader.GetString("City");
                userDetails.State = reader.GetString("State");
                userDetails.ZipCode = reader.GetString("ZipCode");
                userDetails.CreatedAt = reader.GetDateTime("CreatedAt");
                userDetails.UpdatedAt = reader.IsDBNull("UpdatedAt") ? (DateTime?)null : reader.GetDateTime("UpdatedAt");
            }

            // Fetch associated vehicles
            var vehicles = new List<VehicleResponseDto>();
            if (await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    vehicles.Add(new VehicleResponseDto
                    {
                        VehicleID = reader.GetInt32("VehicleID"),
                        Type = reader.GetString("Type"),
                        Make = reader.GetString("Make"),
                        Model = reader.GetString("Model"),
                        Year = reader.GetInt32("Year"),
                        RegistrationNumber = reader.GetString("RegistrationNumber"),
                        ChassisNumber = reader.GetString("ChassisNumber"),
                        EngineNumber = reader.GetString("EngineNumber"),
                        Color = reader.GetString("Color"),
                        FuelType = reader.GetString("FuelType"),
                        MarketValue = reader.GetDecimal("MarketValue"),

                    });
                }
            }
            userDetails.Vehicles = vehicles;

            // Fetch associated policies
            var policies = new List<PolicyResponseDto>();
            if (await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    policies.Add(new PolicyResponseDto
                    {
                        PolicyId = reader.GetInt32("PolicyID"),
                        PolicyType = reader.GetString("PolicyType"),
                        PremiumAmount = reader.GetDecimal("PremiumAmount"),
                        PolicyStartDate = reader.GetDateTime("PolicyStartDate"),
                        PolicyEndDate = reader.GetDateTime("PolicyEndDate"),
                        Status = reader.GetString("Status"),

                    });
                }
            }
            userDetails.Policies = policies;

            // Fetch associated claims
            var claims = new List<ClaimResponseDto>();
            if (await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    claims.Add(new ClaimResponseDto
                    {
                        ClaimId = reader.GetInt32("ClaimID"),
                        PolicyId = reader.GetInt32("PolicyID"),
                        Status = reader.GetString("Status"),
                        IncidentDate = reader.GetDateTime("IncidentDate"),
                        Description = reader.GetString("Description"),
                        ClaimAmount = reader.GetDecimal("ClaimAmount"),
                        ApprovalDate = reader.IsDBNull("ApprovalDate") ? (DateTime?)null : reader.GetDateTime("ApprovalDate"),
                        CreatedAt = reader.GetDateTime("CreatedAt"),
                        UpdatedAt = reader.IsDBNull("UpdatedAt") ? (DateTime?)null : reader.GetDateTime("UpdatedAt")
                    });
                }
            }
            userDetails.Claims = claims;

            return userDetails;
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserID == userId);
        }

        public async Task UpdateUserRoleAsync(User user, string newRole)
        {

            if (user.Role == newRole)
            {
                return;
            }
            user.Role = newRole;
            user.UpdatedAt = DateTime.UtcNow;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
        public async Task<(IEnumerable<AdminClaimResponseDto>, int)> GetAllClaimsForReviewAsync(int pageNumber, int pageSize)
        {
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "GetAllClaimsForReview";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@PageNumber", pageNumber));
            command.Parameters.Add(new SqlParameter("@PageSize", pageSize));

            await _context.Database.OpenConnectionAsync();

            using var reader = await command.ExecuteReaderAsync();
            var claims = new List<AdminClaimResponseDto>();
            int totalCount = 0;

            // Fetch claims
            while (await reader.ReadAsync())
            {
                claims.Add(new AdminClaimResponseDto
                {
                    ClaimId = reader.GetInt32("ClaimID"),
                    Status = reader.GetString("Status"),
                    IncidentDate = reader.GetDateTime("IncidentDate"),
                    Description = reader.GetString("Description"),
                    ClaimAmount = reader.IsDBNull("ClaimAmount") ? null : reader.GetDecimal("ClaimAmount"),
                    ApprovalDate = reader.IsDBNull("ApprovalDate") ? null : reader.GetDateTime("ApprovalDate"),
                    CreatedAt = reader.GetDateTime("CreatedAt"),
                    Policy = new AdminPolicyDetailsDto
                    {
                        PolicyId = reader.GetInt32("PolicyID"),
                        PolicyType = reader.GetString("PolicyType"),
                        PolicyStartDate = reader.GetDateTime("PolicyStartDate"),
                        PolicyEndDate = reader.GetDateTime("PolicyEndDate")
                    },
                    User = new AdminUserDetailsDto
                    {
                        UserId = reader.GetInt32("UserID"),
                        Name = reader.GetString("UserName"),
                        Email = reader.GetString("UserEmail")
                    }
                });
            }

            // Fetch total count
            if (await reader.NextResultAsync() && await reader.ReadAsync())
            {
                totalCount = reader.GetInt32("TotalCount");
            }

            return (claims, totalCount);
        }
        public async Task<(IEnumerable<AdminPolicyResponseDto>, int)> GetAllPoliciesAsync(int pageNumber, int pageSize)
        {
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "GetAllPolicies";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@PageNumber", pageNumber));
            command.Parameters.Add(new SqlParameter("@PageSize", pageSize));

            await _context.Database.OpenConnectionAsync();

            using var reader = await command.ExecuteReaderAsync();
            var policies = new List<AdminPolicyResponseDto>();
            int totalCount = 0;

            // Fetch policies
            while (await reader.ReadAsync())
            {
                policies.Add(new AdminPolicyResponseDto
                {
                    PolicyId = reader.GetInt32("PolicyID"),
                    PolicyType = reader.GetString("PolicyType"),
                    CoverageAmount = reader.GetDecimal("CoverageAmount"),
                    PremiumAmount = reader.GetDecimal("PremiumAmount"),
                    PolicyStartDate = reader.GetDateTime("PolicyStartDate"),
                    PolicyEndDate = reader.GetDateTime("PolicyEndDate"),
                    Status = reader.GetString("Status"),
                    CreatedAt = reader.GetDateTime("CreatedAt"),
                    UpdatedAt = reader.IsDBNull("UpdatedAt") ? null : reader.GetDateTime("UpdatedAt"),
                    User = new AdminAllUserDetailsDto
                    {
                        UserId = reader.GetInt32("UserID"),
                        Name = reader.GetString("UserName"),
                        Email = reader.GetString("UserEmail")
                    },
                    Vehicle = new AdminVehicleDetailsDto
                    {
                        VehicleId = reader.GetInt32("VehicleID"),
                        Make = reader.GetString("VehicleMake"),
                        Model = reader.GetString("VehicleModel"),
                        RegistrationNumber = reader.GetString("RegistrationNumber")
                    }
                });
            }

            // Fetch total count
            if (await reader.NextResultAsync() && await reader.ReadAsync())
            {
                totalCount = reader.GetInt32("TotalCount");
            }

            return (policies, totalCount);
        }

    }
}
