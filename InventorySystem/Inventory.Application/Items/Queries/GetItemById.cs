using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Application.Items.Queries;

public sealed record GetItemById(Guid Id) : IRequest<ItemDto?>;

public sealed class GetItemByIdHandler(IInventoryDbContext db) : IRequestHandler<GetItemById, ItemDto?>
{
    public async Task<ItemDto?> Handle(GetItemById req, CancellationToken ct) =>
        await db.Items.AsNoTracking()
            .Where(x => x.Id == req.Id)
            .Select(x => new ItemDto(x.Id, x.Sku, x.Name, x.Quantity, x.Location, x.MinStock, x.UpdatedAt))
            .FirstOrDefaultAsync(ct);
}
