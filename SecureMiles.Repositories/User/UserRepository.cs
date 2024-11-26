using System.Threading.Tasks;
using SecureMiles.Models;
using Microsoft.EntityFrameworkCore;
using SecureMiles.Common.Data;
using Microsoft.Data.SqlClient;

namespace SecureMiles.Repositories
{
    public class UserRepository : IUserRepository
    {
        // The InsuranceContext is used to interact with the database
        private readonly InsuranceContext _context;

        public UserRepository(InsuranceContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email); // Returns null if no match
        }


        public async Task<User> GetUserByIdAsync(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserID == userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found.");
            }
            return user;
        }

        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
        public async Task<User> UpdateUserProfileAsync(int userId, User updatedUserDetails)
        {
            var parameters = new[]
            {
        new SqlParameter("@UserID", userId),
        new SqlParameter("@Name", updatedUserDetails.Name),
        new SqlParameter("@Address", updatedUserDetails.Address),
        new SqlParameter("@City", updatedUserDetails.City),
        new SqlParameter("@State", updatedUserDetails.State),
        new SqlParameter("@ZipCode", updatedUserDetails.ZipCode),
        new SqlParameter("@Phone", updatedUserDetails.Phone),
        new SqlParameter("@DOB", updatedUserDetails.DOB),
        new SqlParameter("@AadhaarNumber", updatedUserDetails.AadhaarNumber),
        new SqlParameter("@PAN", updatedUserDetails.PAN)
    };

            var result = await _context.Users
                .FromSqlRaw("EXEC UpdateUserProfile @UserID, @Name, @Address, @City, @State, @ZipCode, @Phone, @DOB, @AadhaarNumber,@PAN", parameters)
                .ToListAsync();

            var updatedUser = result.FirstOrDefault();
            if (updatedUser == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found after update.");
            }
            return updatedUser; // Return the updated user
        }


        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task AddResetTokenAsync(PasswordResetToken token)
        {
            await _context.PasswordResetTokens.AddAsync(token);
            await _context.SaveChangesAsync();
        }
        public async Task<PasswordResetToken?> GetValidTokenAsync(string token)
        {
            return await _context.PasswordResetTokens
                .FirstOrDefaultAsync(t => t.Token == token && !t.IsUsed && t.ExpiryTime > DateTime.UtcNow);
        }

        public async Task MarkTokenAsUsedAsync(PasswordResetToken token)
        {
            token.IsUsed = true;
            _context.PasswordResetTokens.Update(token);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateUserPasswordAsync(int userId, string hashedPassword)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserID == userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            user.PasswordHash = hashedPassword;
            user.UpdatedAt = DateTime.UtcNow;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }





    }
}