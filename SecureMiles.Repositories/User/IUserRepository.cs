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


    }
}
