using JsonWebToken.Users.Infrastructure;
using JsonWebToken.Users.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using JsonWebToken.Database;

namespace JsonWebToken.Users
{
    public sealed class LoginUser(AppDbContext context, PasswordHasher<User> passwordHasher, TokenProvider tokenProvider)
    {
        public sealed record Request(string Email, string Password);

        public async Task<string> Handle(Request request)
        {
            User? user = await context.Users.GetByEmail(request.Email);
            if (user is null || !user.EmailVerified)
            {
                throw new Exception("The user was not found");
            }

            var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                throw new Exception("The password is incorrect");
            }

            var token = tokenProvider.Create(user);
            return token;
        }
    }
}