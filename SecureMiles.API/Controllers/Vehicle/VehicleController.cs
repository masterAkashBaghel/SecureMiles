using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using SecureMiles.Common.DTOs.Vehicle;
using SecureMiles.Services.Vehicle;


namespace SecureMiles.API.Controllers.Vehicle
{
    [ApiController]
    [Route("api/[controller]")]

    public class VehicleController : ControllerBase
    {

        private readonly ILogger<UserController> _logger;
        private readonly IVehicleService _vehicleService;

        public VehicleController(ILogger<UserController> logger, IVehicleService vehicleService)
        {
            _logger = logger;
            _vehicleService = vehicleService;
        }

        [HttpPost]
        [Authorize] // Ensure the user is authenticated
        public async Task<IActionResult> AddVehicle([FromBody] AddVehicleRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid vehicle request: {Request}", request);
                return BadRequest(ModelState); // Return validation errors
            }

            try
            {
                // Retrieve User ID from claims
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out var userId))
                {
                    _logger.LogWarning("Invalid user ID claim: {UserIdClaim}", userIdClaim);
                    return Unauthorized(new { Error = "Invalid user ID." });
                }

                // Add vehicle and fetch policy options
                var policyOptions = await _vehicleService.AddVehicleAsync(userId, request);

                return Ok(new
                {
                    Message = "Vehicle added successfully.",
                    Policies = policyOptions // Return policy options in the response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding vehicle.");
                return StatusCode(500, new { Error = "An unexpected error occurred." });
            }
        }

        [HttpGet]
        [Authorize] // Ensure the user is authenticated
        public async Task<IActionResult> GetVehicles()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null)
                {
                    _logger.LogWarning("User ID claim is null.");
                    return Unauthorized(new { Error = "Invalid user ID." });
                }
                var userId = int.Parse(userIdClaim); // Get User ID from JWT
                var vehicles = await _vehicleService.GetVehiclesByUserIdAsync(userId);

                if (vehicles == null || vehicles.Count() == 0)
                {
                    return NotFound(new { Message = "No vehicles found for this user." });
                }

                return Ok(vehicles); // Return the list of vehicles
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving vehicles for user.");
                return StatusCode(500, new { Error = "An unexpected error occurred." });
            }
        }

        [HttpGet("{vehicleId}")]
        [Authorize] // Ensure the user is authenticated
        public async Task<IActionResult> GetVehicleDetails(int vehicleId)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null)
                {
                    _logger.LogWarning("User ID claim is null.");
                    return Unauthorized(new { Error = "Invalid user ID." });
                }
                var userId = int.Parse(userIdClaim); // Extract UserID from JWT
                var vehicleDetails = await _vehicleService.GetVehicleDetailsAsync(vehicleId, userId);

                return Ok(vehicleDetails); // Return the response DTO
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Vehicle not found or unauthorized access for VehicleId: {VehicleId}.", vehicleId);
                return NotFound(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving vehicle details for VehicleId: {VehicleId}.", vehicleId);
                return StatusCode(500, new { Error = "An unexpected error occurred." });
            }
        }



        [HttpPut("{vehicleId}")]
        [Authorize]
        public async Task<IActionResult> UpdateVehicle(int vehicleId, [FromBody] UpdateVehicleRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid vehicle update request: {Request}", request);
                return BadRequest(ModelState); // Return validation errors
            }

            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null)
                {
                    _logger.LogWarning("User ID claim is null.");
                    return Unauthorized(new { Error = "Invalid user ID." });
                }
                var userId = int.Parse(userIdClaim); // Extract UserID from JWT
                await _vehicleService.UpdateVehicleAsync(vehicleId, userId, request);
                return Ok(new { Message = "Vehicle information updated successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access or vehicle not found: {VehicleId}", vehicleId);
                return NotFound(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating vehicle information for {VehicleId}", vehicleId);
                return StatusCode(500, new { Error = "An unexpected error occurred." });
            }
        }



        [HttpDelete("{vehicleId}")]
        [Authorize] // Ensure the user is authenticated
        public async Task<IActionResult> DeleteVehicle(int vehicleId)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null)
                {
                    _logger.LogWarning("User ID claim is null.");
                    return Unauthorized(new { Error = "Invalid user ID." });
                }
                var userId = int.Parse(userIdClaim); // Extract UserID from JWT
                bool result = await _vehicleService.DeleteVehicleAsync(vehicleId, userId);

                if (result)
                {
                    return Ok(new { Message = "Vehicle deleted successfully." });
                }

                return NotFound(new { Error = "Vehicle not found or already deleted." });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access or vehicle not found: {VehicleId}", vehicleId);
                return NotFound(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting vehicle {VehicleId}", vehicleId);
                return StatusCode(500, new { Error = "An unexpected error occurred." });
            }
        }

        // get user vehicles by type 
        [HttpGet("type/{vehicleType}")]
        [Authorize] // Ensure the user is authenticated
        public async Task<IActionResult> GetVehiclesByType(string vehicleType)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null)
                {
                    _logger.LogWarning("User ID claim is null.");
                    return Unauthorized(new { Error = "Invalid user ID." });
                }
                var userId = int.Parse(userIdClaim); // Extract UserID from JWT
                var vehicles = await _vehicleService.GetVehiclesByTypeAsync(userId, vehicleType);

                if (vehicles == null || vehicles.Count() == 0)
                {
                    return NotFound(new { Message = "No vehicles found for this user." });
                }

                return Ok(vehicles); // Return the list of vehicles
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving vehicles for user.");
                return StatusCode(500, new { Error = "An unexpected error occurred." });
            }
        }

    }
}
