using Inventory.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Application.Items.Queries;

public record ItemDto(Guid Id, string Sku, string Name, int Quantity, string Location, int MinStock, DateTime UpdatedAt);
public record GetItemById(Guid Id) : IRequest<ItemDto?>;

public class GetItemByIdHandler(IInventoryDbContext db) : IRequestHandler<GetItemById, ItemDto?>
{
    public async Task<ItemDto?> Handle(GetItemById req, CancellationToken ct) =>
        await db.Items.Where(x => x.Id == req.Id)
            .Select(x => new ItemDto(x.Id, x.Sku, x.Name, x.Quantity, x.Location, x.MinStock, x.UpdatedAt))
            .FirstOrDefaultAsync(ct);
}
