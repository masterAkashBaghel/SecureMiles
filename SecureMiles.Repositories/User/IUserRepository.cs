using SecureMiles.Models;
using System.Threading.Tasks;

namespace SecureMiles.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUserByEmailAsync(string email);
        Task AddUserAsync(User user);

    }
}
