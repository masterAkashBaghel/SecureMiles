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
                userDetails.Phone = reader.GetString("Phone");
                userDetails.Email = reader.GetString("Email");
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

        // method to get total policy,user,claim and proposals without using stored procedure

        public async Task<DashboardDataResponseDto> GetDashboardDataAsync()
        {
            var totalUsers = await _context.Users.CountAsync();
            var totalPolicies = await _context.Policies.CountAsync();
            var totalClaims = await _context.Claims.CountAsync();
            var totalProposals = await _context.Proposals.CountAsync();

            return new DashboardDataResponseDto
            {
                TotalUsers = totalUsers,
                TotalPolicies = totalPolicies,
                TotalClaims = totalClaims,
                TotalProposals = totalProposals
            };
        }

        // method to get all proposals for review without using stored procedure

        public async Task<PaginatedProposalsResponseDto> GetAllProposalsAsync(int pageNumber, int pageSize)
        {
            var query = _context.Proposals
                .Include(p => p.Policy)
                .Include(p => p.User)
                .OrderBy(p => p.CreatedAt);

            var totalCount = await query.CountAsync();
            var proposals = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProposalResponseDto
                {
                    ProposalId = p.ProposalID,
                    PolicyName = p.Policy.Type,
                    UserName = p.User.Name,
                    UserEmail = p.User.Email,
                    UserPhone = p.User.Phone,
                    Status = p.Status,
                    UserId = p.UserID,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();

            return new PaginatedProposalsResponseDto
            {
                Proposals = proposals,
                TotalCount = totalCount,
                CurrentPage = pageNumber,
                PageSize = pageSize
            };
        }
        // method to get all details of a proposal without using stored procedure

        public async Task<ProposalDetailsResponseDto?> GetProposalDetailsAsync(int proposalId)
        {
            var proposal = await _context.Proposals
                .Include(p => p.Policy)
                .Include(p => p.User)
                .Include(p => p.Vehicle)
                .Include(p => p.Documents)
                .FirstOrDefaultAsync(p => p.ProposalID == proposalId);

            if (proposal == null)
            {
                return null;
            }

            return new ProposalDetailsResponseDto
            {
                ProposalId = proposal.ProposalID,
                UserId = proposal.UserID,
                UserName = proposal.User.Name,
                UserEmail = proposal.User.Email,
                UserPhone = proposal.User.Phone,
                Status = proposal.Status,
                CreatedAt = proposal.CreatedAt,
                ApprovalDate = proposal.ApprovalDate,
                SubmissionDate = proposal.SubmissionDate,
                RequestedCoverage = proposal.RequestedCoverage,
                Vehicle = new VehicleDto
                {
                    VehicleId = proposal.Vehicle.VehicleID,
                    Make = proposal.Vehicle.Make,
                    Model = proposal.Vehicle.Model,
                    Year = proposal.Vehicle.Year,
                },
                Documents = proposal.Documents
            .Where(d => d.ProposalID == proposalId)
            .Select(d => new DocumentDto
            {
                DocumentId = d.DocumentID,
                DocumentType = d.Type,
                DocumentUrl = d.FilePath
            }).ToList()
            };
        }

        // method to delete a proposal by admin without using stored procedure

        public async Task DeleteProposalAsync(int proposalId)
        {
            var proposal = await _context.Proposals.FindAsync(proposalId);
            if (proposal == null)
            {
                throw new KeyNotFoundException("Proposal not found.");
            }

            _context.Proposals.Remove(proposal);
            await _context.SaveChangesAsync();

        }

        // method to approve  proposal by admin

        public async Task ApproveProposalAsync(int proposalId)
        {
            var proposal = await _context.Proposals.FindAsync(proposalId);
            if (proposal == null)
            {
                throw new KeyNotFoundException("Proposal not found.");
            }

            proposal.Status = "Approved";
            proposal.ApprovalDate = DateTime.UtcNow;
            _context.Proposals.Update(proposal);
            await _context.SaveChangesAsync();



        }
    }
}
