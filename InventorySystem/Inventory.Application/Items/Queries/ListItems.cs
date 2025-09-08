using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Application.Items.Queries;

public sealed record ListItems(string? Search, int Page, int PageSize) : IRequest<List<ItemDto>>;

public sealed record ItemDto(Guid Id, string Sku, string Name, int Quantity, string Location, int MinStock, DateTime UpdatedAt);

public sealed class ListItemsHandler(IInventoryDbContext db) : IRequestHandler<ListItems, List<ItemDto>>
{
    public async Task<List<ItemDto>> Handle(ListItems req, CancellationToken ct)
    {
        var q = db.Items.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(req.Search))
        {
            var s = req.Search.Trim();
            q = q.Where(x => x.Sku.Contains(s) || x.Name.Contains(s));
        }

        return await q
            .OrderByDescending(x => x.UpdatedAt)
            .Skip((req.Page - 1) * req.PageSize)
            .Take(req.PageSize)
            .Select(x => new ItemDto(x.Id, x.Sku, x.Name, x.Quantity, x.Location, x.MinStock, x.UpdatedAt))
            .ToListAsync(ct);
    }
}
