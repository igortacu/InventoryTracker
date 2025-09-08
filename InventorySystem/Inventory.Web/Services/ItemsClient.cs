using System.Net.Http.Json;
using Inventory.Web.Models;

namespace Inventory.Web.Services;

public class ItemsClient(HttpClient http)
{
    public async Task<IReadOnlyList<ItemDto>> ListAsync(string? search, int page = 1, int pageSize = 20)
    {
        var url = $"items?search={Uri.EscapeDataString(search ?? "")}&page={page}&pageSize={pageSize}";
        var res = await http.GetFromJsonAsync<List<ItemDto>>(url);
        return res ?? [];
    }

    public async Task<Guid?> CreateAsync(CreateItemRequest req)
    {
        var resp = await http.PostAsJsonAsync("items", req);
        if (!resp.IsSuccessStatusCode) return null;
        var data = await resp.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        return data is not null && data.TryGetValue("id", out var idStr) ? Guid.Parse(idStr) : null;
    }

    public Task<ItemDto?> GetAsync(Guid id) =>
        http.GetFromJsonAsync<ItemDto>($"items/{id}");

    public async Task<bool> UpdateAsync(Guid id, string? name, string? location, int? minStock)
    {
        var body = new { name, location, minStock };
        var resp = await http.PutAsJsonAsync($"items/{id}", body);
        return resp.IsSuccessStatusCode;
    }

    public async Task<int?> AdjustAsync(Guid id, int delta)
    {
        var resp = await http.PostAsync($"items/{id}/adjust?delta={delta}", null);
        if (!resp.IsSuccessStatusCode) return null;
        var data = await resp.Content.ReadFromJsonAsync<Dictionary<string, int>>();
        return data is not null && data.TryGetValue("quantity", out var q) ? q : null;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var resp = await http.DeleteAsync($"items/{id}");
        return resp.IsSuccessStatusCode;
    }
}
