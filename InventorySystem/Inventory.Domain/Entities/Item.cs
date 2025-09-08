namespace Inventory.Domain.Entities;

public class Item
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Sku { get; set; } = default!;
    public string Name { get; set; } = default!;
    public int Quantity { get; set; }
    public string Location { get; set; } = "MAIN";
    public int MinStock { get; set; } = 0;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
