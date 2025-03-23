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

// Veritabaný baðlantý dizesini al
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// DbContext'i ekleyin - sadece bir kez tanýmlayýn
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

// MVC ve API desteði ekleyin
builder.Services.AddControllersWithViews();
builder.Services.AddControllers();

// TokenProvider'ý ekleyin
builder.Services.AddSingleton<TokenProvider>();

// Swagger ekleyin (isteðe baðlý)
// builder.Services.AddSwaggerGen();

var app = builder.Build();

// Geliþtirme ortamýnda hata sayfalarý göster
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    // Swagger'ý yalnýzca development ortamýnda etkinleþtirin (isteðe baðlý)
    // app.UseSwagger();
    // app.UseSwaggerUI();
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

// Routing'i etkinleþtirin
app.UseRouting();

// Kimlik doðrulama ve yetkilendirme
app.UseAuthentication();
app.UseAuthorization();

// Endpoint yapýlandýrmasý
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllers();

// Swagger URL'sine doðrudan eriþim engelleme (eðer Swagger kullanýyorsanýz)
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