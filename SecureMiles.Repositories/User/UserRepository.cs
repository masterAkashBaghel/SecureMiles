using System.Threading.Tasks;
using SecureMiles.Models;
using Microsoft.EntityFrameworkCore;
using SecureMiles.Common.Data;

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
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with email {email} not found.");
            }
            return user;
        }

        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
    }
}