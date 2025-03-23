using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using JsonWebToken.Users.Infrastructure.Models;
using JsonWebToken.Database;
using BCrypt.Net;
using JsonWebToken.Users.Infrastructure;
using JsonWebToken.Users;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly TokenProvider _tokenProvider;
    private readonly LoginUser _loginUser;

    public AuthController(AppDbContext context, TokenProvider tokenProvider, LoginUser loginUser)
    {
        _context = context;
        _tokenProvider = tokenProvider;
        _loginUser = loginUser;
    }

    // Kullanıcı Kaydı (Register)
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUser registerUser)
    {
        // Kullanıcı adı veya e-posta zaten kullanılıyor mu kontrol et
        if (await _context.Users.AnyAsync(u => u.Username == registerUser.Username))
        {
            return BadRequest("Kullanıcı adı zaten alınmış.");
        }

        if (await _context.Users.AnyAsync(u => u.Email == registerUser.Email))
        {
            return BadRequest("E-posta zaten kayıtlı.");
        }

        // Şifreyi hash'le
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerUser.Password);

        var user = new User
        {
            Username = registerUser.Username,
            Email = registerUser.Email,
            PasswordHash = passwordHash,
            EmailVerified = false // E-posta doğrulama işlemi yapılmadı
        };

        // Kullanıcıyı veritabanına ekle
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Token üret
        var token = _tokenProvider.Create(user);

        return Ok(new { token });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUser.Request request)
    {
        try
        {
            var token = await _loginUser.Handle(request);
            return Ok(new { token });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

}