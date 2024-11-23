USE SecureMilesDB;
GO

CREATE PROCEDURE GetVehicleDetails
    @VehicleId INT,
    @UserId INT
AS
BEGIN
    -- Fetch vehicle details
    SELECT
        v.VehicleID, v.Make, v.Model, v.Year, v.RegistrationNumber,
        v.Type AS VehicleType, v.Color, v.FuelType, v.MarketValue,
        v.CreatedAt, v.UpdatedAt
    FROM Vehicles v
    WHERE v.VehicleID = @VehicleId AND v.UserID = @UserId;

    -- Fetch associated proposals
    SELECT
        p.ProposalID, p.Status, p.SubmissionDate, p.ApprovalDate
    FROM Proposals p
    WHERE p.VehicleID = @VehicleId;

    -- Fetch associated policies
    SELECT
        po.PolicyID, po.Type AS PolicyType, po.PremiumAmount, po.PolicyStartDate, po.PolicyEndDate, po.Status
    FROM Policies po
    WHERE po.VehicleID = @VehicleId;
END;