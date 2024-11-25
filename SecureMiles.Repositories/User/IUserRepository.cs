using SecureMiles.Models;
using System.Threading.Tasks;

namespace SecureMiles.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUserByEmailAsync(string email);
        Task AddUserAsync(User user);

        Task<User> UpdateUserProfileAsync(int userId, User updatedUser);

        Task<User> GetUserByIdAsync(int userId);

        Task UpdateUserAsync(User user);

        Task AddResetTokenAsync(PasswordResetToken token);
        Task<PasswordResetToken?> GetValidTokenAsync(string token);

        Task MarkTokenAsUsedAsync(PasswordResetToken token);


        Task UpdateUserPasswordAsync(int userId, string hashedPassword);




    }
}
