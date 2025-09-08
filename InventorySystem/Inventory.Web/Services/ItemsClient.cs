using System.Net.Http.Json;
using Inventory.Web.Models;

namespace Inventory.Web.Services;

public class ItemsClient(HttpClient http)
{
    public async Task<ApiResult<IReadOnlyList<ItemDto>>> ListAsync(string? search, int page = 1, int pageSize = 20)
    {
        try
        {
            var url = $"items?search={Uri.EscapeDataString(search ?? "")}&page={page}&pageSize={pageSize}";
            var res = await http.GetAsync(url);
            if (!res.IsSuccessStatusCode) return ApiResult<IReadOnlyList<ItemDto>>.Fail($"HTTP {(int)res.StatusCode}");
            var data = await res.Content.ReadFromJsonAsync<List<ItemDto>>() ?? [];
            return ApiResult<IReadOnlyList<ItemDto>>.Success(data);
        }
        catch (Exception ex) { return ApiResult<IReadOnlyList<ItemDto>>.Fail(ex.Message); }
    }

    public async Task<ApiResult<Guid>> CreateAsync(CreateItemRequest req)
    {
        try
        {
            var resp = await http.PostAsJsonAsync("items", req);
            if (!resp.IsSuccessStatusCode) return ApiResult<Guid>.Fail($"HTTP {(int)resp.StatusCode}");
            var payload = await resp.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            if (payload is null || !payload.TryGetValue("id", out var id)) return ApiResult<Guid>.Fail("Invalid response");
            return Guid.TryParse(id, out var g) ? ApiResult<Guid>.Success(g) : ApiResult<Guid>.Fail("Bad id");
        }
        catch (Exception ex) { return ApiResult<Guid>.Fail(ex.Message); }
    }

    public async Task<ApiResult<bool>> UpdateAsync(Guid id, string? name, string? location, int? minStock)
    {
        try
        {
            var resp = await http.PutAsJsonAsync($"items/{id}", new { name, location, minStock });
            return resp.IsSuccessStatusCode ? ApiResult<bool>.Success(true) : ApiResult<bool>.Fail($"HTTP {(int)resp.StatusCode}");
        }
        catch (Exception ex) { return ApiResult<bool>.Fail(ex.Message); }
    }

    public async Task<ApiResult<int>> AdjustAsync(Guid id, int delta)
    {
        try
        {
            var resp = await http.PostAsync($"items/{id}/adjust?delta={delta}", null);
            if (!resp.IsSuccessStatusCode) return ApiResult<int>.Fail($"HTTP {(int)resp.StatusCode}");
            var data = await resp.Content.ReadFromJsonAsync<Dictionary<string, int>>();
            return data is not null && data.TryGetValue("quantity", out var q)
                ? ApiResult<int>.Success(q) : ApiResult<int>.Fail("Invalid response");
        }
        catch (Exception ex) { return ApiResult<int>.Fail(ex.Message); }
    }

    public async Task<ApiResult<bool>> DeleteAsync(Guid id)
    {
        try
        {
            var resp = await http.DeleteAsync($"items/{id}");
            return resp.IsSuccessStatusCode ? ApiResult<bool>.Success(true) : ApiResult<bool>.Fail($"HTTP {(int)resp.StatusCode}");
        }
        catch (Exception ex) { return ApiResult<bool>.Fail(ex.Message); }
    }
}
