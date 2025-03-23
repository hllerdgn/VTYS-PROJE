using Microsoft.EntityFrameworkCore;
using JsonWebToken.Users.Infrastructure.Models;

namespace JsonWebToken.Users.Infrastructure
{
    public static class UserExtensions
    {
        public static async Task<User?> GetByEmail(this DbSet<User> users, string email)
        {
            return await users.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}