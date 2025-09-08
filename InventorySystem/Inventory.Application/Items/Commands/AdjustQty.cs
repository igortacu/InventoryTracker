using Inventory.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Application.Items.Commands;

public record AdjustQty(Guid Id, int Delta) : IRequest<int?>;

public class AdjustQtyHandler(IInventoryDbContext db) : IRequestHandler<AdjustQty, int?>
{
    public async Task<int?> Handle(AdjustQty req, CancellationToken ct)
    {
        var e = await db.Items.FirstOrDefaultAsync(x => x.Id == req.Id, ct);
        if (e is null) return null;

        e.Quantity += req.Delta;
        e.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);
        return e.Quantity;
    }
}
