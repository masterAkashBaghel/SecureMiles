USE SecureMilesDB;
GO

ALTER PROCEDURE UpdateUserProfile
    @UserID INT,
    @Name NVARCHAR(100),
    @Address NVARCHAR(255),
    @City NVARCHAR(50),
    @State NVARCHAR(50),
    @ZipCode NVARCHAR(10),
    @Phone NVARCHAR(15),
    @DOB DATE,
    @AadhaarNumber NVARCHAR(12),
    @PAN NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Check if the user exists
        IF EXISTS (SELECT 1
    FROM Users
    WHERE UserID = @UserID)
        BEGIN
        -- Update user profile
        UPDATE Users
            SET 
                Name = @Name,
                Address = @Address,
                City = @City,
                State = @State,
                ZipCode = @ZipCode,
                Phone = @Phone,
                DOB = @DOB,
                AadhaarNumber = @AadhaarNumber,
                PAN = @PAN,
                UpdatedAt = GETDATE()
            WHERE UserID = @UserID;

        -- Return the updated user profile
        SELECT *
        FROM Users
        WHERE UserID = @UserID;
    END
        ELSE
        BEGIN
        -- Return an error message if the user does not exist
        RAISERROR('User with UserID %d does not exist.', 16, 1, @UserID);
    END
    END TRY
    BEGIN CATCH
        -- Handle errors
        DECLARE @ErrorMessage NVARCHAR(4000);
        DECLARE @ErrorSeverity INT;
        DECLARE @ErrorState INT;

        SELECT
        @ErrorMessage = ERROR_MESSAGE(),
        @ErrorSeverity = ERROR_SEVERITY(),
        @ErrorState = ERROR_STATE();

        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH;
END;
GO


SELECT *
FROM Users
 


 
