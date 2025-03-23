using JsonWebToken.Database;
using JsonWebToken.Users;
using JsonWebToken.Users.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using JsonWebToken.Users.Infrastructure.Models;

var builder = WebApplication.CreateBuilder(args);

// Veritaban� ba�lant� dizesini al
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// DbContext'i ekleyin - sadece bir kez tan�mlay�n
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

// Identity servislerini ekleyin
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// MVC ve API deste�i ekleyin
builder.Services.AddControllersWithViews();
builder.Services.AddControllers();

// TokenProvider'� ekleyin
builder.Services.AddSingleton<TokenProvider>();

// Swagger ekleyin (iste�e ba�l�)
// builder.Services.AddSwaggerGen();

var app = builder.Build();

// Geli�tirme ortam�nda hata sayfalar� g�ster
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    // Swagger'� yaln�zca development ortam�nda etkinle�tirin (iste�e ba�l�)
    // app.UseSwagger();
    // app.UseSwaggerUI();
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

// Routing'i etkinle�tirin
app.UseRouting();

// Kimlik do�rulama ve yetkilendirme
app.UseAuthentication();
app.UseAuthorization();

// Endpoint yap�land�rmas�
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllers();

// Swagger URL'sine do�rudan eri�im engelleme (e�er Swagger kullan�yorsan�z)
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