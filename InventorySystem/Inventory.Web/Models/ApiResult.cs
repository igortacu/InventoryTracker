namespace Inventory.Web.Models;

public sealed record ApiResult<T>(bool Ok, T? Data, string? Error)
{
    public static ApiResult<T> Success(T data) => new(true, data, null);
    public static ApiResult<T> Fail(string error) => new(false, default, error);
}
