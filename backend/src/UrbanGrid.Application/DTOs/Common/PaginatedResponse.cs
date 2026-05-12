namespace UrbanGrid.Application.DTOs.Common;

public class ApiResponse<T>
{
    public string Status { get; set; } = "success";
    public T? Data { get; set; }
    public string? Message { get; set; }

    public static ApiResponse<T> Success(T data, string? message = null) =>
        new() { Status = "success", Data = data, Message = message };

    public static ApiResponse<T> Error(string message) =>
        new() { Status = "error", Message = message };
}

public class PaginationMeta
{
    public int Total { get; set; }
    public int Page { get; set; }
    public int Limit { get; set; }
    public int Pages => (int)Math.Ceiling((double)Total / Limit);
}
