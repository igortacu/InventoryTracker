using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Application.Items.Commands;

public sealed record DeleteItem(Guid Id) : IRequest<bool>;

public sealed class DeleteItemHandler(IInventoryDbContext db) : IRequestHandler<DeleteItem, bool>
{
    public async Task<bool> Handle(DeleteItem req, CancellationToken ct)
    {
        var it = await db.Items.FirstOrDefaultAsync(x => x.Id == req.Id, ct);
        if (it is null) return false;
        db.Items.Remove(it);
        await db.SaveChangesAsync(ct);
        return true;
    }
}
