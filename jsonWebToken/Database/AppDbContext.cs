using Microsoft.EntityFrameworkCore;
using JsonWebToken.Users.Infrastructure.Models;


namespace JsonWebToken.Database;

public class AppDbContext : DbContext
{
    // Kullanıcılar tablosunu temsil eden DbSet
    public DbSet<User> Users { get; set; }

    // Constructor
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Veritabanı modeli oluşturulurken yapılandırma
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Örnek bir yapılandırma: Kullanıcı tablosu için benzersiz e-posta kısıtlaması
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }
}