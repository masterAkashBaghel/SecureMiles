USE SecureMilesDB;
GO

CREATE PROCEDURE UpdateClaimInformation
    @ClaimId INT,
    @UserId INT,
    @IsAdmin BIT,
    @Status NVARCHAR(20) = NULL,
    @Description NVARCHAR(1000) = NULL,
    @ClaimAmount DECIMAL(18, 2) = NULL,
    @UpdatedAt DATETIME = NULL
AS
BEGIN
    -- Validate claim existence
    IF NOT EXISTS (SELECT 1
    FROM Claims
    WHERE ClaimId = @ClaimId)
    BEGIN
        RAISERROR('Claim not found.', 16, 1);
    END

    -- Ensure only admins can update status
    IF @Status IS NOT NULL AND @IsAdmin = 0
    BEGIN
        RAISERROR('Only admins can update claim status.', 16, 1);
    END

    -- Update the claim
    UPDATE Claims
    SET 
        Status = COALESCE(@Status, Status),
        Description = COALESCE(@Description, Description),
        ClaimAmount = COALESCE(@ClaimAmount, ClaimAmount),
        UpdatedAt = @UpdatedAt
    WHERE ClaimId = @ClaimId;

    SELECT
        ClaimId, PolicyId, Status, Description, ClaimAmount, UpdatedAt
    FROM Claims
    WHERE ClaimId = @ClaimId;
END;
GO
