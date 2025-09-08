namespace Inventory.Domain;

public class Item
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = "";
    public string Name { get; set; } = "";
    public int Quantity { get; set; }
    public string Location { get; set; } = "";
    public int MinStock { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
