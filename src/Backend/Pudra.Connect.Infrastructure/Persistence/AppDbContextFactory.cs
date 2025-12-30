using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Pudra.Connect.Infrastructure.Persistence;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        // ÖNEMLİ: Bu bölümü tekrar dosya okuyacak şekilde veya geçici olarak SQL Server bağlantı cümlenle güncelle.
        // Şimdilik hard-coded bırakıyorum.
        var connectionString =
            "Server=49.13.200.112;Database=PudraConnectDb;User Id=app;Password=y9U%8L9LCQRy9YM$; Trusted_Connection=false;TrustServerCertificate=True;MultipleActiveResultSets=true;Connect Timeout=600;";
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        // DEĞİŞİKLİK BURADA:
        optionsBuilder.UseSqlServer(connectionString); 
        return new AppDbContext(optionsBuilder.Options);
    }
}