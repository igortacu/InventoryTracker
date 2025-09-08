using Inventory.Application.Common;
using Inventory.Domain.Entities;
using MediatR;

namespace Inventory.Application.Items.Commands;

public record CreateItem(string Sku, string Name, int Quantity, string? Location, int? MinStock) : IRequest<Guid>;

public class CreateItemHandler(IInventoryDbContext db) : IRequestHandler<CreateItem, Guid>
{
    public async Task<Guid> Handle(CreateItem req, CancellationToken ct)
    {
        var e = new Item
        {
            Sku = req.Sku.Trim(),
            Name = req.Name.Trim(),
            Quantity = req.Quantity,
            Location = string.IsNullOrWhiteSpace(req.Location) ? "MAIN" : req.Location!.Trim(),
            MinStock = req.MinStock ?? 0
        };

        db.Items.Add(e);
        await db.SaveChangesAsync(ct);
        return e.Id;
    }
}
