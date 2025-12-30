using Microsoft.EntityFrameworkCore;
using Pudra.Connect.Domain.Entities;

namespace Pudra.Connect.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
 
    
    // Not: Sale ve Return entity'lerini eklediğimizde DbSet'lerini buraya ekleyeceğiz.

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Veritabanı şeması için ek kurallar ve yapılandırmalar burada yapılır.
        // Örneğin, Username'in benzersiz (unique) olmasını sağlayalım.
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        base.OnModelCreating(modelBuilder);
    }
}

