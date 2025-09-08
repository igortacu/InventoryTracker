using Inventory.Domain;
using MediatR;

namespace Inventory.Application.Items.Commands;

public sealed record CreateItem(string Sku, string Name, int Quantity, string? Location, int? MinStock) : IRequest<Guid>;

public sealed class CreateItemHandler(IInventoryDbContext db) : IRequestHandler<CreateItem, Guid>
{
    public async Task<Guid> Handle(CreateItem req, CancellationToken ct)
    {
        var entity = new Item
        {
            Id = Guid.NewGuid(),
            Sku = req.Sku,
            Name = req.Name,
            Quantity = req.Quantity,
            Location = req.Location ?? "",
            MinStock = req.MinStock ?? 0,
            UpdatedAt = DateTime.UtcNow
        };
        db.Items.Add(entity);
        await db.SaveChangesAsync(ct);
        return entity.Id;
    }
}
