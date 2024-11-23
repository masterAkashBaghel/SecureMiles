
using SecureMiles.Common.DTOs.Vehicle;

namespace SecureMiles.Services.Vehicle
{
    public interface IVehicleService
    {
        Task AddVehicleAsync(int userId, AddVehicleRequestDto request);

        Task<List<VehicleResponseDto>> GetVehiclesByUserIdAsync(int userId);

        Task<VehicleDetailsResponseDto> GetVehicleDetailsAsync(int vehicleId, int userId);

        Task<bool> UpdateVehicleAsync(int vehicleId, int userId, UpdateVehicleRequestDto vehicleDto);

        Task<bool> DeleteVehicleAsync(int vehicleId, int userId);

    }
}