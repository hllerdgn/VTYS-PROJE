using Microsoft.AspNetCore.Identity;

namespace JsonWebToken.Users.Infrastructure.Models;

public class User : IdentityUser
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public bool EmailVerified { get; set; }
}