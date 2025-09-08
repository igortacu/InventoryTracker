using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Application.Items.Commands;

public sealed record AdjustQty(Guid Id, int Delta) : IRequest<int?>;

public sealed class AdjustQtyHandler(IInventoryDbContext db) : IRequestHandler<AdjustQty, int?>
{
    public async Task<int?> Handle(AdjustQty req, CancellationToken ct)
    {
        var it = await db.Items.FirstOrDefaultAsync(x => x.Id == req.Id, ct);
        if (it is null) return null;
        it.Quantity += req.Delta;
        it.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return it.Quantity;
    }
}
