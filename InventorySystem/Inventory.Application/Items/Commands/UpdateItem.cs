using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Application.Items.Commands;

public sealed record UpdateItem(Guid Id, string? Name, string? Location, int? MinStock) : IRequest<bool>;

public sealed class UpdateItemHandler(IInventoryDbContext db) : IRequestHandler<UpdateItem, bool>
{
    public async Task<bool> Handle(UpdateItem req, CancellationToken ct)
    {
        var it = await db.Items.FirstOrDefaultAsync(x => x.Id == req.Id, ct);
        if (it is null) return false;
        if (req.Name is not null) it.Name = req.Name;
        if (req.Location is not null) it.Location = req.Location;
        if (req.MinStock is not null) it.MinStock = req.MinStock.Value;
        it.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return true;
    }
}
