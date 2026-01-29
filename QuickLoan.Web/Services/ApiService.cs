using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using QuickLoan.Web.Interfaces;
using QuickLoan.Web.Models;
using Microsoft.Extensions.Logging;

namespace QuickLoan.Web.Services;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ApiService> _logger;

    public ApiService(HttpClient httpClient, IConfiguration configuration, ILogger<ApiService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;

        var apiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "https://localhost:5001";
        _httpClient.BaseAddress = new Uri(apiBaseUrl);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        _logger.LogInformation("ApiService initialized with base URL: {BaseUrl}", apiBaseUrl);
    }

    public async Task<ApiResponse<T>> GetAsync<T>(string endpoint, string? token = null)
    {
        _logger.LogDebug("GET Request to {Endpoint}", endpoint);

        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                _logger.LogDebug("Authorization header added for GET {Endpoint}", endpoint);
            }

            var response = await _httpClient.SendAsync(request);
            _logger.LogInformation("GET {Endpoint} returned {StatusCode}", endpoint, response.StatusCode);

            return await ProcessResponseAsync<T>(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GET request to {Endpoint}", endpoint);
            return new ApiResponse<T>
            {
                Success = false,
                Message = $"Error: {ex.Message}"
            };
        }
    }

    public async Task<ApiResponse<T>> PostAsync<T>(string endpoint, object data, string? token = null)
    {
        _logger.LogDebug("POST Request to {Endpoint} with data: {Data}", endpoint, JsonConvert.SerializeObject(data));

        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json")
            };

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                _logger.LogDebug("Authorization header added for POST {Endpoint}", endpoint);
            }

            var response = await _httpClient.SendAsync(request);
            _logger.LogInformation("POST {Endpoint} returned {StatusCode}", endpoint, response.StatusCode);

            return await ProcessResponseAsync<T>(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in POST request to {Endpoint}", endpoint);
            return new ApiResponse<T>
            {
                Success = false,
                Message = $"Error: {ex.Message}"
            };
        }
    }

    public async Task<ApiResponse<T>> PutAsync<T>(string endpoint, object data, string? token = null)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Put, endpoint)
            {
                Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json")
            };

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.SendAsync(request);
            return await ProcessResponseAsync<T>(response);
        }
        catch (Exception ex)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = $"Error: {ex.Message}"
            };
        }
    }

    public async Task<ApiResponse<T>> DeleteAsync<T>(string endpoint, string? token = null)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, endpoint);
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.SendAsync(request);
            return await ProcessResponseAsync<T>(response);
        }
        catch (Exception ex)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = $"Error: {ex.Message}"
            };
        }
    }

    private async Task<ApiResponse<T>> ProcessResponseAsync<T>(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<T>>(content);
            return apiResponse ?? new ApiResponse<T> { Success = false, Message = "Invalid response" };
        }
        else
        {
            // Try to parse error response
            try
            {
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(content);
                return new ApiResponse<T>
                {
                    Success = false,
                    Message = errorResponse?.Message ?? $"Error: {response.StatusCode}"
                };
            }
            catch
            {
                return new ApiResponse<T>
                {
                    Success = false,
                    Message = $"Error: {response.StatusCode} - {content}"
                };
            }
        }
    }
}

public class ErrorResponse
{
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; }
    public int StatusCode { get; set; }
}