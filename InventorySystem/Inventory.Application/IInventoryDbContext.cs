using Inventory.Domain;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Application;

public interface IInventoryDbContext
{
    DbSet<Item> Items { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
