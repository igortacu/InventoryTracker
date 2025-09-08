using Inventory.Application.Common;
using MediatR;

namespace Inventory.Application.Items.Commands;

public record DeleteItem(Guid Id) : IRequest<bool>;

public class DeleteItemHandler(IInventoryDbContext db) : IRequestHandler<DeleteItem, bool>
{
    public async Task<bool> Handle(DeleteItem req, CancellationToken ct)
    {
        var e = await db.Items.FindAsync([req.Id], ct);
        if (e is null) return false;

        db.Items.Remove(e);
        await db.SaveChangesAsync(ct);
        return true;
    }
}
