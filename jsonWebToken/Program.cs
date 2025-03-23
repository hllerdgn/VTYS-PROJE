using JsonWebToken.Database;
using JsonWebToken.Users;
using JsonWebToken.Users.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Veritaban� ba�lant� dizesini al
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// DbContext'i ekleyin
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]))
    };
});

// MVC ve API deste�i ekleyin
builder.Services.AddControllersWithViews();
builder.Services.AddControllers();

// TokenProvider'� ekleyin
builder.Services.AddSingleton<TokenProvider>();

var app = builder.Build();

// Geli�tirme ortam�nda hata sayfalar� g�ster
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// HTTPS y�nlendirmesini etkinle�tirin
app.UseHttpsRedirection();

// Statik dosyalar� sunun (CSS, JS, resimler)
app.UseStaticFiles();

// Routing'i etkinle�tirin (sadece bir kez)
app.UseRouting();

// Kimlik do�rulama ve yetkilendirme
app.UseAuthentication();
app.UseAuthorization();

// Endpoint yap�land�rmas� - Sadece bir kez tan�mlay�n
app.UseEndpoints(endpoints =>
{
    // MVC i�in varsay�lan y�nlendirme 
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    // API controller'lar i�in
    endpoints.MapControllers();
});

// Swagger URL'sine do�rudan eri�im engelleme
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/swagger"))
    {
        context.Response.Redirect("/");
        return;
    }

    await next();
});

app.Run();