using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Inventory.Infrastructure;

public class DesignTimeFactory : IDesignTimeDbContextFactory<InventoryDbContext>
{
    public InventoryDbContext CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory();

        // Try both common locations when you run `dotnet ef` from root or from project
        var candidates = new[]
        {
            Path.Combine(basePath, "Inventory.Api", "appsettings.json"),
            Path.Combine(basePath, "..", "Inventory.Api", "appsettings.json"),
        };

        var cfgBuilder = new ConfigurationBuilder().SetBasePath(basePath);

        foreach (var p in candidates)
            if (File.Exists(p))
            {
                cfgBuilder.AddJsonFile(p, optional: true);
                break;
            }

        cfgBuilder.AddEnvironmentVariables();
        var cfg = cfgBuilder.Build();

        var connString =
            cfg.GetConnectionString("Default")
            ?? Environment.GetEnvironmentVariable("ConnectionStrings__Default")
            ?? "Server=localhost,1433;Database=InventoryDb;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=true;";

        var options = new DbContextOptionsBuilder<InventoryDbContext>()
            .UseSqlServer(connString)
            .Options;

        return new InventoryDbContext(options);
    }
}
