namespace Inventory.Web.Models;

public class CreateItemRequest
{
    public string Sku { get; set; } = "";
    public string Name { get; set; } = "";
    public int Quantity { get; set; }
    public string? Location { get; set; }
    public int? MinStock { get; set; }
}
