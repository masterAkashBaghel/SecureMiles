USE SecureMilesDB;
GO

CREATE PROCEDURE GetAllClaimsForReview
    @PageNumber INT,
    @PageSize INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Fetch claims with related details
    SELECT
        c.ClaimID, c.Status, c.IncidentDate, c.Description,
        ISNULL(c.ClaimAmount, 0) AS ClaimAmount, -- Handle NULL ClaimAmount
        c.ApprovalDate, c.CreatedAt, c.UpdatedAt,
        p.PolicyID, p.Type AS PolicyType, p.PolicyStartDate, p.PolicyEndDate,
        u.UserID, u.Name AS UserName, u.Email AS UserEmail
    FROM Claims c
        INNER JOIN Policies p ON c.PolicyID = p.PolicyID
        INNER JOIN Users u ON p.UserID = u.UserID
    ORDER BY c.CreatedAt
    OFFSET (@PageNumber - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;

    -- Return total count of claims
    SELECT COUNT(*) AS TotalCount
    FROM Claims;
END;
GO

EXEC GetAllClaimsForReview 1, 10;