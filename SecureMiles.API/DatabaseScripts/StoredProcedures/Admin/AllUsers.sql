
USE SecureMilesDB;
GO

ALTER PROCEDURE dbo.GetUserDetails
    @UserId INT
AS
BEGIN
    -- Fetch user details
    SELECT
        u.UserID, u.Name, u.Address, u.City, u.State,
        u.ZipCode, u.CreatedAt, u.UpdatedAt
    FROM Users u
    WHERE u.UserID = @UserId;

    -- Fetch associated vehicles
    SELECT
        v.VehicleID, v.Type, v.Make, v.Model, v.Year, v.RegistrationNumber,
        v.ChassisNumber, v.EngineNumber, v.Color, v.FuelType,
        ISNULL(v.MarketValue, 0) AS MarketValue, -- Handle NULL MarketValue
        v.CreatedAt, v.UpdatedAt
    FROM Vehicles v
    WHERE v.UserID = @UserId;

    -- Fetch associated policies
    SELECT
        p.PolicyID, p.Type AS PolicyType, p.CoverageAmount, p.PremiumAmount,
        p.PolicyStartDate, p.PolicyEndDate, p.Status, p.CreatedAt, p.UpdatedAt,
        p.ProposalID
    FROM Policies p
    WHERE p.UserID = @UserId;

    -- Fetch associated claims
    SELECT
        c.ClaimID, c.PolicyID, c.Status, c.IncidentDate, c.Description,
        ISNULL(c.ClaimAmount, 0) AS ClaimAmount, -- Handle NULL ClaimAmount
        c.ApprovalDate, c.CreatedAt, c.UpdatedAt
    FROM Claims c
    WHERE c.PolicyID IN (SELECT PolicyID
    FROM Policies
    WHERE UserID = @UserId);
END;
GO
 

 