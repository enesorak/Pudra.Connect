using Microsoft.EntityFrameworkCore;
using Pudra.Connect.Domain.Entities;

namespace Pudra.Connect.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<ScanLog> ScanLogs { get; set; } // YENİ

    
    // Not: Sale ve Return entity'lerini eklediğimizde DbSet'lerini buraya ekleyeceğiz.

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Veritabanı şeması için ek kurallar ve yapılandırmalar burada yapılır.
        // Örneğin, Username'in benzersiz (unique) olmasını sağlayalım.
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();
        
        
        // ScanLog indexes
        modelBuilder.Entity<ScanLog>()
            .HasIndex(s => s.UserId);
        
        modelBuilder.Entity<ScanLog>()
            .HasIndex(s => s.Barcode);
        
        modelBuilder.Entity<ScanLog>()
            .HasIndex(s => s.ScannedAt);

        modelBuilder.Entity<ScanLog>()
            .HasOne(s => s.User)
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
    }
}

