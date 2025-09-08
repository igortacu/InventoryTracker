using Inventory.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Application.Items.Commands;

public record UpdateItem(Guid Id, string? Name, string? Location, int? MinStock) : IRequest<bool>;

public class UpdateItemHandler(IInventoryDbContext db) : IRequestHandler<UpdateItem, bool>
{
    public async Task<bool> Handle(UpdateItem req, CancellationToken ct)
    {
        var e = await db.Items.FirstOrDefaultAsync(x => x.Id == req.Id, ct);
        if (e is null) return false;

        if (!string.IsNullOrWhiteSpace(req.Name)) e.Name = req.Name!.Trim();
        if (!string.IsNullOrWhiteSpace(req.Location)) e.Location = req.Location!.Trim();
        if (req.MinStock is not null) e.MinStock = req.MinStock.Value;
        e.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);
        return true;
    }
}
