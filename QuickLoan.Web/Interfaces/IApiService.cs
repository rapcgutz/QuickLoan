using QuickLoan.Web.Models;

namespace QuickLoan.Web.Interfaces;

public interface IApiService
{
    Task<ApiResponse<T>> GetAsync<T>(string endpoint, string? token = null);
    Task<ApiResponse<T>> PostAsync<T>(string endpoint, object data, string? token = null);
    Task<ApiResponse<T>> PutAsync<T>(string endpoint, object data, string? token = null);
    Task<ApiResponse<T>> DeleteAsync<T>(string endpoint, string? token = null);
}

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
}
