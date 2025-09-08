using Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Application.Common;

public interface IInventoryDbContext
{
    DbSet<Item> Items { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
