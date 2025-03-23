using JsonWebToken.Database;
using JsonWebToken.Users;
using JsonWebToken.Users.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Veritabaný baðlantý dizesini al
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

// MVC ve API desteði ekleyin
builder.Services.AddControllersWithViews();
builder.Services.AddControllers();

// TokenProvider'ý ekleyin
builder.Services.AddSingleton<TokenProvider>();

var app = builder.Build();

// Geliþtirme ortamýnda hata sayfalarý göster
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// HTTPS yönlendirmesini etkinleþtirin
app.UseHttpsRedirection();

// Statik dosyalarý sunun (CSS, JS, resimler)
app.UseStaticFiles();

// Routing'i etkinleþtirin (sadece bir kez)
app.UseRouting();

// Kimlik doðrulama ve yetkilendirme
app.UseAuthentication();
app.UseAuthorization();

// Endpoint yapýlandýrmasý - Sadece bir kez tanýmlayýn
app.UseEndpoints(endpoints =>
{
    // MVC için varsayýlan yönlendirme 
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    // API controller'lar için
    endpoints.MapControllers();
});

// Swagger URL'sine doðrudan eriþim engelleme
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