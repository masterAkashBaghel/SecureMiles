
using SecureMiles.Common.DTOs.Policy;
using SecureMiles.Common.DTOs.Vehicle;

namespace SecureMiles.Services.Vehicle
{
    public interface IVehicleService
    {
        Task<IEnumerable<PolicyOptionDto>> AddVehicleAsync(int userId, AddVehicleRequestDto request);
        Task<List<VehicleResponseDto>> GetVehiclesByUserIdAsync(int userId);

        Task<VehicleDetailsResponseDto> GetVehicleDetailsAsync(int vehicleId, int userId);

        Task<bool> UpdateVehicleAsync(int vehicleId, int userId, UpdateVehicleRequestDto vehicleDto);

        Task<bool> DeleteVehicleAsync(int vehicleId, int userId);

        IEnumerable<PolicyOptionDto> GeneratePolicyOptions(Models.Vehicle vehicle, decimal marketValue);

        Task<List<VehicleResponseDto>> GetVehiclesByTypeAsync(int userId, string type);
    }
}