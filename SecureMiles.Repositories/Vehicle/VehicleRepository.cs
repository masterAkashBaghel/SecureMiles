

using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SecureMiles.Common.Data;
using SecureMiles.Common.DTOs.Policy;
using SecureMiles.Common.DTOs.Proposals;
using SecureMiles.Common.DTOs.Vehicle;

namespace SecureMiles.Repositories.Vehicle
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly InsuranceContext _context;


        public VehicleRepository(InsuranceContext context)
        {
            _context = context;
        }

        public async Task AddVehicleAsync(int userId, SecureMiles.Models.Vehicle vehicle)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserID == userId);
            if (user == null)
            {

                throw new KeyNotFoundException($"User with ID {userId} not found.");
            }

            vehicle.UserID = userId;
            await _context.Vehicles.AddAsync(vehicle);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Models.Vehicle>> GetVehiclesByUserIdAsync(int userId)
        {
            return await _context.Vehicles
                .Where(v => v.UserID == userId && v.IsActive)
                .ToListAsync();
        }




        public async Task<VehicleDetailsResponseDto> GetVehicleDetailsAsync(int vehicleId, int userId)
        {
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "GetVehicleDetails";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@VehicleId", vehicleId));
            command.Parameters.Add(new SqlParameter("@UserId", userId));

            await _context.Database.OpenConnectionAsync();

            using var reader = await command.ExecuteReaderAsync();
            // Initialize the response object
            var vehicleDetails = new VehicleDetailsResponseDto();

            // Fetch vehicle details
            if (await reader.ReadAsync())
            {
                vehicleDetails.VehicleId = reader.GetInt32("VehicleID");
                vehicleDetails.Make = reader.GetString("Make");
                vehicleDetails.Model = reader.GetString("Model");
                vehicleDetails.Year = reader.GetInt32("Year");
                vehicleDetails.RegistrationNumber = reader.GetString("RegistrationNumber");
                vehicleDetails.VehicleType = reader.GetString("VehicleType");
                vehicleDetails.Color = reader.GetString("Color");
                vehicleDetails.FuelType = reader.GetString("FuelType");
                vehicleDetails.MarketValue = reader.GetDecimal("MarketValue");
                vehicleDetails.CreatedAt = reader.GetDateTime("CreatedAt");
                vehicleDetails.UpdatedAt = reader.IsDBNull("UpdatedAt") ? (DateTime?)null : reader.GetDateTime("UpdatedAt");
            }

            // Fetch proposals
            var proposals = new List<ProposalResponseDto>();
            if (await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    proposals.Add(new ProposalResponseDto
                    {
                        ProposalId = reader.GetInt32("ProposalID"),
                        Status = reader.GetString("Status"),
                        SubmissionDate = reader.GetDateTime("SubmissionDate"),
                        ApprovalDate = reader.IsDBNull("ApprovalDate") ? (DateTime?)null : reader.GetDateTime("ApprovalDate")
                    });
                }
            }
            vehicleDetails.Proposals = proposals;

            // Fetch policies
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
                        Status = reader.GetString("Status")
                    });
                }
            }
            vehicleDetails.Policies = policies;

            return vehicleDetails;
        }



        public async Task<bool> UpdateVehicleAsync(int vehicleId, int userId, UpdateVehicleRequestDto vehicleDto)
        {
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "UpdateVehicleDetails";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@VehicleId", vehicleId));
            command.Parameters.Add(new SqlParameter("@UserId", userId));
            command.Parameters.Add(new SqlParameter("@Make", vehicleDto.Make ?? (object)DBNull.Value));
            command.Parameters.Add(new SqlParameter("@Model", vehicleDto.Model ?? (object)DBNull.Value));
            command.Parameters.Add(new SqlParameter("@Year", vehicleDto.Year));
            command.Parameters.Add(new SqlParameter("@RegistrationNumber", vehicleDto.RegistrationNumber ?? (object)DBNull.Value));
            command.Parameters.Add(new SqlParameter("@VehicleType", vehicleDto.VehicleType ?? (object)DBNull.Value));
            command.Parameters.Add(new SqlParameter("@Color", vehicleDto.Color ?? (object)DBNull.Value));
            command.Parameters.Add(new SqlParameter("@FuelType", vehicleDto.FuelType ?? (object)DBNull.Value));
            command.Parameters.Add(new SqlParameter("@MarketValue", vehicleDto.MarketValue));
            command.Parameters.Add(new SqlParameter("@UpdatedAt", DateTime.UtcNow));

            await _context.Database.OpenConnectionAsync();

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var rowsAffected = reader.GetInt32("RowsAffected");
                return rowsAffected > 0;
            }

            return false;
        }



        public async Task<bool> DeleteVehicleAsync(int vehicleId, int userId)
        {
            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.VehicleID == vehicleId && v.UserID == userId && v.IsActive);

            if (vehicle == null)
            {
                return false; // Vehicle not found or already deleted
            }

            // Soft delete the vehicle
            vehicle.IsActive = false;
            vehicle.UpdatedAt = DateTime.UtcNow;

            _context.Vehicles.Update(vehicle);
            await _context.SaveChangesAsync();

            return true; // Successfully deleted
        }
        public async Task<Models.Vehicle?> GetVehicleEntityAsync(int vehicleId, int userId)
        {
            var vehicle = await _context.Vehicles
                .Include(v => v.User) // Include User relationship
                .FirstOrDefaultAsync(v => v.VehicleID == vehicleId && v.UserID == userId && v.IsActive);

            if (vehicle == null)
            {
                return null;
            }

            return vehicle;
        }


        // get vehicle by type
        public async Task<List<Models.Vehicle>> GetVehiclesByTypeAsync(int userId, string type)
        {
            return await _context.Vehicles
                .Where(v => v.UserID == userId && v.IsActive && v.Type == type)
                .ToListAsync();
        }
    }
}