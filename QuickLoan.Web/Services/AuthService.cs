using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using QuickLoan.Web.Interfaces;
using QuickLoan.Web.Models;

namespace QuickLoan.Web.Services;

public class AuthService : IAuthService
{
    private readonly IApiService _apiService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(IApiService apiService, IHttpContextAccessor httpContextAccessor)
    {
        _apiService = apiService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var response = await _apiService.PostAsync<LoginResponse>("/api/auth/login", request);
        
        if (response.Success && response.Data != null)
        {
            await CreateAuthenticationCookieAsync(response.Data.AccessToken, response.Data.RefreshToken, 
                response.Data.User.Email, response.Data.Role, response.Data.User.FirstName, response.Data.User.LastName);
            return response.Data;
        }

        return response.Data;
    }

    public async Task<ApiResponse<RegisterResponse>> RegisterAsync(RegisterRequest request)
    {
        var response = await _apiService.PostAsync<RegisterResponse>("/api/auth/register", request);

        if (response.Success && response.Data != null)
        {
            await CreateAuthenticationCookieAsync(
                response.Data.AccessToken,
                response.Data.RefreshToken,
                response.Data.User.Email,
                response.Data.Role,
                response.Data.User.FirstName,
                response.Data.User.LastName
            );
        }

        return response; 
    }

    public async Task<TokenResponse?> RefreshTokenAsync(string refreshToken)
    {
        var response = await _apiService.PostAsync<TokenResponse>("/api/auth/refresh", new { refreshToken });
        
        if (response.Success && response.Data != null)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                var email = httpContext.User.FindFirst(ClaimTypes.Email)?.Value ?? "";
                var role = httpContext.User.FindFirst(ClaimTypes.Role)?.Value ?? "";
                var firstName = httpContext.User.FindFirst(ClaimTypes.GivenName)?.Value ?? "";
                var lastName = httpContext.User.FindFirst(ClaimTypes.Surname)?.Value ?? "";

                await CreateAuthenticationCookieAsync(response.Data.AccessToken, response.Data.RefreshToken,
                    email, role, firstName, lastName);
            }
            
            return response.Data;
        }

        return response.Data;
    }

    public async Task LogoutAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext != null)
        {
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext != null && httpContext.User.Identity?.IsAuthenticated == true)
        {
            return httpContext.User.FindFirst("AccessToken")?.Value;
        }

        return null;
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        return await Task.FromResult(httpContext?.User.Identity?.IsAuthenticated == true);
    }

    private async Task CreateAuthenticationCookieAsync(string accessToken, string refreshToken, 
        string email, string role, string firstName, string lastName)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) return;

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role),
            new Claim(ClaimTypes.GivenName, firstName),
            new Claim(ClaimTypes.Surname, lastName),
            new Claim("AccessToken", accessToken),
            new Claim("RefreshToken", refreshToken)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
        };

        await httpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);
    }
}
