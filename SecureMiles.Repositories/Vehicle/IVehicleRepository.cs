using SecureMiles.Common.DTOs.Vehicle;
using SecureMiles.Models;
using System.Threading.Tasks;

namespace SecureMiles.Repositories.Vehicle
{
    public interface IVehicleRepository
    {
        Task AddVehicleAsync(int userId, Models.Vehicle vehicle);

        Task<List<Models.Vehicle>> GetVehiclesByUserIdAsync(int userId);

        Task<VehicleDetailsResponseDto> GetVehicleDetailsAsync(int vehicleId, int userId);

        Task<bool> UpdateVehicleAsync(int vehicleId, int userId, UpdateVehicleRequestDto vehicleDto);
        Task<bool> DeleteVehicleAsync(int vehicleId, int userId);


    }
}