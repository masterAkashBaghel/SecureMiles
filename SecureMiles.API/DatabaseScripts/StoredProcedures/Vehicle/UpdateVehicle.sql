use SecureMilesDB;
GO

CREATE PROCEDURE UpdateVehicleDetails
    @VehicleId INT,
    @UserId INT,
    @Make NVARCHAR(100),
    @Model NVARCHAR(100),
    @Year INT,
    @RegistrationNumber NVARCHAR(20),
    @VehicleType NVARCHAR(50),
    @Color NVARCHAR(30),
    @FuelType NVARCHAR(50),
    @MarketValue DECIMAL(18, 2),
    @UpdatedAt DATETIME
AS
BEGIN
    -- Update vehicle details only if the user owns the vehicle
    UPDATE Vehicles
    SET 
        Make = @Make,
        Model = @Model,
        Year = @Year,
        RegistrationNumber = @RegistrationNumber,
        Type = @VehicleType,
        Color = @Color,
        FuelType = @FuelType,
        MarketValue = @MarketValue,
        UpdatedAt = @UpdatedAt
    WHERE VehicleID = @VehicleId AND UserID = @UserId;

    -- Return the number of rows affected
    SELECT @@ROWCOUNT AS RowsAffected;
END;
GO
