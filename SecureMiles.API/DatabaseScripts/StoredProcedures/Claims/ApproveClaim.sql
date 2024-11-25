USE SecureMilesDB;
GO

CREATE PROCEDURE ApproveClaim
    @ClaimId INT,
    @ApprovedAmount DECIMAL(18, 2),
    @Notes NVARCHAR(500),
    @ApprovalDate DATETIME
AS
BEGIN
    -- Validate claim existence and status
    IF NOT EXISTS (SELECT 1
    FROM Claims
    WHERE ClaimId = @ClaimId AND Status IN ('Pending', 'UnderReview'))
    BEGIN
        RAISERROR('Claim not found or not in a valid state for approval.', 16, 1);
    END

    -- Approve the claim
    UPDATE Claims
    SET 
        Status = 'Approved',
        ClaimAmount = @ApprovedAmount,
        ApprovalDate = @ApprovalDate,
        UpdatedAt = @ApprovalDate
     WHERE ClaimId = @ClaimId;

    SELECT
        ClaimId, PolicyId, Status, ClaimAmount AS ApprovedAmount, ApprovalDate
    FROM Claims
    WHERE ClaimId = @ClaimId;
END;
GO
