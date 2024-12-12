

using System.Data;
using Microsoft.EntityFrameworkCore;
using SecureMiles.Common.Data;
using SecureMiles.Models;

namespace SecureMiles.Repositories.Proposals
{
    public class ProposalsRepository : IProposalsRepository
    {
        private readonly InsuranceContext _context;

        public ProposalsRepository(InsuranceContext context)
        {
            _context = context;
        }

        public async Task<int> AddProposalAsync(Proposal proposal)
        {
            await _context.Proposals.AddAsync(proposal);
            await _context.SaveChangesAsync();
            return proposal.ProposalID;
        }



        public async Task<Proposal> GetProposalByIdAsync(int proposalId)
        {
            var proposal = await _context.Proposals
                .FirstOrDefaultAsync(p => p.ProposalID == proposalId);

            if (proposal == null)
            {
                throw new KeyNotFoundException($"Proposal with ID {proposalId} not found.");
            }

            return proposal;
        }

        public async Task<List<Proposal>> GetProposalsByVehicleIdAsync(int vehicleId)
        {
            return await _context.Proposals
                .Where(p => p.VehicleID == vehicleId)
                .ToListAsync();
        }
        public async Task<List<Proposal>> GetProposalsByUserIdAsync(int userId)
        {
            return await _context.Proposals
                .Include(p => p.Vehicle) // Include vehicle details
                .Where(p => p.UserID == userId && p.Status != "Canceled") // Filter out canceled proposals
                .OrderByDescending(p => p.SubmissionDate) // Order by latest proposals
                .ToListAsync();
        }

        public async Task<Proposal> GetProposalByIdAsync(int proposalId, int userId)
        {
            var proposal = await _context.Proposals
                .Include(p => p.Vehicle) // Include vehicle details
                .FirstOrDefaultAsync(p => p.ProposalID == proposalId && p.UserID == userId);

            if (proposal == null)
            {
                throw new KeyNotFoundException($"Proposal with ID {proposalId} and UserID {userId} not found.");
            }

            return proposal;
        }
        public async Task<bool> CancelProposalAsync(int proposalId, int userId)
        {
            var proposal = await _context.Proposals.FirstOrDefaultAsync(p => p.ProposalID == proposalId && p.UserID == userId);

            if (proposal == null)
            {
                return false; // Proposal not found or unauthorized access
            }

            if (proposal.Status != "Pending")
            {
                throw new InvalidOperationException("Only pending proposals can be canceled.");
            }

            proposal.Status = "Canceled"; // Mark the proposal as canceled
            proposal.UpdatedAt = DateTime.UtcNow;

            _context.Proposals.Update(proposal);
            await _context.SaveChangesAsync();

            return true;
        }


        // method to update the proposal status
        public async Task<bool> UpdateProposalAsync(Proposal proposal)
        {
            _context.Proposals.Update(proposal);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
