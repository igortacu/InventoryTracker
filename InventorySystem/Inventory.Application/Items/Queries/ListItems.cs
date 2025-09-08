using Inventory.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Application.Items.Queries;

public record ListItems(string? Search, int Page = 1, int PageSize = 20) : IRequest<IReadOnlyList<ItemDto>>;

public class ListItemsHandler(IInventoryDbContext db) : IRequestHandler<ListItems, IReadOnlyList<ItemDto>>
{
    public async Task<IReadOnlyList<ItemDto>> Handle(ListItems req, CancellationToken ct)
    {
        var q = db.Items.AsQueryable();

        if (!string.IsNullOrWhiteSpace(req.Search))
            q = q.Where(x => x.Sku.Contains(req.Search!) || x.Name.Contains(req.Search!));

        return await q
            .OrderBy(x => x.Sku)
            .Skip((req.Page - 1) * req.PageSize)
            .Take(req.PageSize)
            .Select(x => new ItemDto(x.Id, x.Sku, x.Name, x.Quantity, x.Location, x.MinStock, x.UpdatedAt))
            .ToListAsync(ct);
    }
}
