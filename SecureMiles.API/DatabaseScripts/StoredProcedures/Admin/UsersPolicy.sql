USE SecureMilesDB;
GO

CREATE PROCEDURE GetAllPolicies
    @PageNumber INT,
    @PageSize INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Fetch policies with user and vehicle details
    SELECT
        p.PolicyID, p.Type AS PolicyType, p.CoverageAmount, p.PremiumAmount,
        p.PolicyStartDate, p.PolicyEndDate, p.Status, p.CreatedAt, p.UpdatedAt,
        p.ProposalID,
        u.UserID, u.Name AS UserName, u.Email AS UserEmail,
        v.VehicleID, v.Make AS VehicleMake, v.Model AS VehicleModel, v.RegistrationNumber
    FROM Policies p
        INNER JOIN Users u ON p.UserID = u.UserID
        INNER JOIN Vehicles v ON p.VehicleID = v.VehicleID
    ORDER BY p.CreatedAt
    OFFSET (@PageNumber - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;

    -- Return total count of policies
    SELECT COUNT(*) AS TotalCount
    FROM Policies;
END;
GO

exec GetAllPolicies 1, 10;