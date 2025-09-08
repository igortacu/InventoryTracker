using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Inventory.Infrastructure;

public class DesignTimeFactory : IDesignTimeDbContextFactory<InventoryDbContext>
{
    public InventoryDbContext CreateDbContext(string[] args)
    {
        var cfg = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var cs = cfg.GetConnectionString("Default")
                 ?? cfg["ConnectionStrings:Default"]
                 ?? "Server=localhost,1433;Database=InventoryDb;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=true;";

        var opt = new DbContextOptionsBuilder<InventoryDbContext>()
            .UseSqlServer(cs)
            .Options;

        return new InventoryDbContext(opt);
    }
}
