using SecureMiles.Models;
using Microsoft.EntityFrameworkCore;
using SecureMiles.Common.Data;
using SecureMiles.Common.DTOs.Claims;
using System.Data;
using Microsoft.Data.SqlClient;

namespace SecureMiles.Repositories.Claims
{
    public class ClaimRepository : IClaimRepository
    {
        private readonly InsuranceContext _context;

        public ClaimRepository(InsuranceContext context)
        {
            _context = context;
        }
        public async Task<int> AddClaimAsync(Claim claim)
        {
            await _context.Claims.AddAsync(claim);
            await _context.SaveChangesAsync();
            return claim.ClaimID;
        }
        public async Task<List<Claim>> GetClaimsByUserIdAsync(int userId)
        {
            return await _context.Claims
                .Include(c => c.Policy) // Include related policy
                .Include(c => c.Documents) // Include associated documents
                .Where(c => c.Policy.UserID == userId) // Ensure the user owns the claims
                .OrderByDescending(c => c.CreatedAt) // Order by the most recent claims
                .ToListAsync();
        }

        public async Task<Claim?> GetClaimByIdAsync(int claimId, int userId)
        {
            return await _context.Claims
                .Include(c => c.Policy) // Include policy details
                .ThenInclude(p => p.Vehicle) // Include vehicle details from policy
                .Include(c => c.Documents) // Include associated documents
                .FirstOrDefaultAsync(c => c.ClaimID == claimId && c.Policy.UserID == userId);
        }
        public async Task<UpdateClaimResponseDto> UpdateClaimAsync(int claimId, int userId, bool isAdmin, UpdateClaimRequestDto request)
        {
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "UpdateClaimInformation";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@ClaimId", claimId));
            command.Parameters.Add(new SqlParameter("@UserId", userId));
            command.Parameters.Add(new SqlParameter("@IsAdmin", isAdmin));
            command.Parameters.Add(new SqlParameter("@Status", request.Status ?? (object)DBNull.Value));
            command.Parameters.Add(new SqlParameter("@Description", request.Description ?? (object)DBNull.Value));
            command.Parameters.Add(new SqlParameter("@ClaimAmount", request.ClaimAmount ?? (object)DBNull.Value));
            command.Parameters.Add(new SqlParameter("@UpdatedAt", DateTime.UtcNow));

            await _context.Database.OpenConnectionAsync();

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new UpdateClaimResponseDto
                {
                    ClaimId = reader.GetInt32("ClaimId"),
                    PolicyId = reader.GetInt32("PolicyId"),
                    Status = reader.GetString("Status"),
                    Description = reader.GetString("Description"),
                    ClaimAmount = reader.IsDBNull("ClaimAmount") ? null : reader.GetDecimal("ClaimAmount"),
                    UpdatedAt = reader.GetDateTime("UpdatedAt")
                };
            }

            throw new KeyNotFoundException("Claim update failed.");
        }
        public async Task<ApproveClaimResponseDto> ApproveClaimAsync(int claimId, ApproveClaimRequestsDto request)
        {
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "ApproveClaim";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@ClaimId", claimId));
            command.Parameters.Add(new SqlParameter("@ApprovedAmount", request.ApprovedAmount));
            command.Parameters.Add(new SqlParameter("@ApprovalDate", DateTime.UtcNow));
            command.Parameters.Add(new SqlParameter("@Notes", request.Notes ?? (object)DBNull.Value));

            await _context.Database.OpenConnectionAsync();

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new ApproveClaimResponseDto
                {
                    ClaimId = reader.GetInt32("ClaimId"),
                    PolicyId = reader.GetInt32("PolicyId"),
                    Status = reader.GetString("Status"),
                    ApprovedAmount = reader.GetDecimal("ApprovedAmount"),
                    ApprovalDate = reader.GetDateTime("ApprovalDate"),
                };
            }

            throw new KeyNotFoundException("Claim approval failed.");
        }
        public async Task<Claim> RejectClaimAsync(int claimId, string notes)
        {
            var claim = await _context.Claims
                .Include(c => c.Policy) // Include associated policy for validation
                .FirstOrDefaultAsync(c => c.ClaimID == claimId);

            if (claim == null)
            {
                throw new KeyNotFoundException("Claim not found.");
            }

            if (claim.Status != "Pending" && claim.Status != "UnderReview")
            {
                throw new InvalidOperationException("Only claims that are pending or under review can be rejected.");
            }

            // Update the claim status to rejected and set rejection date
            claim.Status = "Rejected";
            claim.UpdatedAt = DateTime.UtcNow;
            claim.Description = notes ?? "Claim rejected.";

            _context.Claims.Update(claim);
            await _context.SaveChangesAsync();

            return claim;
        }

        // method to update a claim
        public async Task<Claim> UpdateClaimAsync(Claim claim)
        {
            _context.Claims.Update(claim);
            await _context.SaveChangesAsync();
            return claim;
        }

        //method to delete a claim in Pending or UnderReview status
        public async Task<Claim> DeleteClaimAsync(int claimId)
        {
            var claim = await _context.Claims
                .Include(c => c.Policy) // Include associated policy for validation
                .FirstOrDefaultAsync(c => c.ClaimID == claimId);

            if (claim == null)
            {
                throw new KeyNotFoundException("Claim not found.");
            }

            if (claim.Status != "Pending" && claim.Status != "UnderReview")
            {
                throw new InvalidOperationException("Only claims that are pending or under review can be deleted.");
            }

            _context.Claims.Remove(claim);
            await _context.SaveChangesAsync();

            return claim;
        }

        // get a cliam by claim id and return  userID

        public async Task<int?> GetClaimByIdAsync(int claimId)
        {
            var claim = await _context.Claims
                .Include(c => c.Policy) // Include associated policy for validation
                 .FirstOrDefaultAsync(c => c.ClaimID == claimId);

            if (claim == null)
            {
                throw new KeyNotFoundException("Claim not found.");
            }

            return claim.Policy.UserID;
        }
    }
}
