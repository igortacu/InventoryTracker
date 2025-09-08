using Inventory.Application;
using Inventory.Domain;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure;

public class InventoryDbContext(DbContextOptions<InventoryDbContext> options)
    : DbContext(options), IInventoryDbContext
{
    public DbSet<Item> Items => Set<Item>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Item>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Sku).HasMaxLength(64).IsRequired();
            e.Property(x => x.Name).HasMaxLength(256).IsRequired();
            e.Property(x => x.Location).HasMaxLength(64);
            e.Property(x => x.MinStock).HasDefaultValue(0);
            e.Property(x => x.UpdatedAt);
            e.HasIndex(x => x.Sku).IsUnique(false);
        });
    }
}
