

using Microsoft.Extensions.Logging;
using SecureMiles.Common.DTOs.Vehicle;
using SecureMiles.Repositories;
using SecureMiles.Repositories.Vehicle;

namespace SecureMiles.Services.Vehicle
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly ILogger<VehicleService> _logger;
        private readonly IUserRepository _userRepository;
        public VehicleService(IVehicleRepository vehicleRepository, ILogger<VehicleService> logger, IUserRepository userRepository)
        {
            _vehicleRepository = vehicleRepository;
            _logger = logger;
            _userRepository = userRepository;
        }

        public async Task AddVehicleAsync(int userId, AddVehicleRequestDto request)
        {
            // Validate the request
            if (request == null)
            {
                _logger.LogWarning("Invalid vehicle request: {Request}", request);
                throw new ArgumentNullException(nameof(request));
            }
            var user = await _userRepository.GetUserByIdAsync(userId);
            // Retrieve the user entity
            if (user == null)
            {
                _logger.LogWarning("User not found: {UserId}", userId);
                throw new ArgumentException("User not found", nameof(userId));
            }

            // Create a new vehicle entity
            var vehicle = new Models.Vehicle
            {
                Make = request.Make,
                Model = request.Model,
                Year = request.Year,
                ChassisNumber = request.ChassisNumber,
                Color = request.Color,
                Type = request.Type,
                RegistrationNumber = request.RegistrationNumber,
                EngineNumber = request.EngineNumber,
                FuelType = request.FuelType,
                User = user,
                PurchaseDate = request.PurchaseDate,
                MarketValue = request.MarketValue,
                Policies = [],
                Proposals = [],
            };

            // Add the vehicle to the database
            await _vehicleRepository.AddVehicleAsync(userId, vehicle);
        }
        public async Task<List<VehicleResponseDto>> GetVehiclesByUserIdAsync(int userId)
        {
            var vehicles = await _vehicleRepository.GetVehiclesByUserIdAsync(userId);
            if (vehicles == null)
            {
                _logger.LogWarning("No vehicles found for user: {UserId}", userId);
                throw new KeyNotFoundException("No vehicles found for this user.");
            }

            // Map the vehicle entities to DTOs
            return vehicles.Select(v => new VehicleResponseDto
            {
                VehicleID = v.VehicleID,
                Make = v.Make,
                Model = v.Model,
                Year = v.Year,
                RegistrationNumber = v.RegistrationNumber,
                Type = v.Type,
                Color = v.Color,
                FuelType = v.FuelType,
                MarketValue = v.MarketValue,
                ChassisNumber = v.ChassisNumber,
                EngineNumber = v.EngineNumber,
                PurchaseDate = v.PurchaseDate,

            }).ToList();
        }
        public async Task<VehicleDetailsResponseDto> GetVehicleDetailsAsync(int vehicleId, int userId)
        {
            var vehicleDetails = await _vehicleRepository.GetVehicleDetailsAsync(vehicleId, userId);

            if (vehicleDetails == null)
            {
                _logger.LogWarning("Vehicle not found or unauthorized access: {VehicleId}", vehicleId);
                throw new KeyNotFoundException("Vehicle not found or unauthorized access.");
            }

            return vehicleDetails;
        }



        public async Task<bool> UpdateVehicleAsync(int vehicleId, int userId, UpdateVehicleRequestDto vehicleDto)
        {
            // Ensure the vehicle exists and belongs to the user
            var vehicle = await _vehicleRepository.GetVehicleDetailsAsync(vehicleId, userId);
            if (vehicle == null)
            {
                throw new KeyNotFoundException("Vehicle not found or unauthorized access.");
            }

            // Update the vehicle
            var updated = await _vehicleRepository.UpdateVehicleAsync(vehicleId, userId, vehicleDto);
            if (!updated)
            {
                throw new InvalidOperationException("Failed to update vehicle information.");
            }

            return updated;
        }



        public async Task<bool> DeleteVehicleAsync(int vehicleId, int userId)
        {
            // Check if the vehicle exists and belongs to the user
            var userVehicle = await _vehicleRepository.GetVehicleDetailsAsync(vehicleId, userId);
            _logger.LogWarning("User Vehicle: {UserVehicle}", userVehicle);
            if (userVehicle == null || !userVehicle.IsActive)
            {
                throw new KeyNotFoundException("Vehicle not found or already deleted.");
            }

            // Perform the soft delete
            var deleted = await _vehicleRepository.DeleteVehicleAsync(vehicleId, userId);
            if (!deleted)
            {
                throw new InvalidOperationException("Failed to delete the vehicle.");
            }

            return deleted;
        }

    }
}


