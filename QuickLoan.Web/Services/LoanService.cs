using QuickLoan.Web.Interfaces;
using QuickLoan.Web.Models;

namespace QuickLoan.Web.Services;

public class LoanService : ILoanService
{
    private readonly IApiService _apiService;
    private readonly IAuthService _authService;
    private readonly ILogger<LoanService> _logger;

    public LoanService(IApiService apiService, IAuthService authService, ILogger<LoanService> logger)
    {
        _apiService = apiService;
        _authService = authService;
        _logger = logger;

    }

    public async Task<ApiResponse<QuotationResponse?>> CalculateQuotationAsync(QuotationRequest request)
    {
        var response = await _apiService.PostAsync<QuotationResponse>("/api/quotation/calculate", request);
        return response;
    }

    public async Task<LoanApplicationResponse?> GetLoanByUrlAsync(string applicationUrl)
    {
        var response = await _apiService.GetAsync<LoanApplicationResponse>($"/api/quotation/loan/{applicationUrl}");
        return response.Success ? response.Data : null;
    }

    public async Task<Guid?> SubmitLoanApplicationAsync(LoanApplicationRequest request)
    {
        try
        {
            _logger.LogInformation("=== SUBMIT LOAN DEBUG ===");

            var token = await _authService.GetAccessTokenAsync();

            if (string.IsNullOrEmpty(token))
            {
                _logger.LogError("No access token available");
                return null;
            }

            _logger.LogInformation("Token available: {TokenPrefix}...", token.Substring(0, Math.Min(20, token.Length)));
            _logger.LogInformation("Submitting to: /api/loanapplication/submit");
            _logger.LogInformation("Request: Amount={Amount}, Term={Term}", request.AmountRequired, request.Term);

            var response = await _apiService.PostAsync<SubmitLoanResponse>("/api/loanapplication/submit", request, token);

            _logger.LogInformation("API Response - Success: {Success}, Message: {Message}", response.Success, response.Message);

            if (!response.Success)
            {
                _logger.LogError("Loan submission failed: {Message}", response.Message);
                return null;
            }

            if (response.Data == null)
            {
                _logger.LogError("Response data is null even though success is true");
                return null;
            }

            _logger.LogInformation("Loan created successfully with ID: {LoanId}", response.Data.Id);
            return response.Data.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during loan submission");
            return null;
        }
    }
    public async Task<List<LoanApplicationResponse>?> GetMyLoansAsync()
    {
        var token = await _authService.GetAccessTokenAsync();
        if (string.IsNullOrEmpty(token))
            return null;

        var response = await _apiService.GetAsync<List<LoanApplicationResponse>>("/api/loanapplication/my-loans", token);
        return response.Success ? response.Data : null;
    }

    public async Task<LoanApplicationResponse?> GetLoanByIdAsync(Guid id)
    {
        var token = await _authService.GetAccessTokenAsync();
        if (string.IsNullOrEmpty(token))
            return null;

        var response = await _apiService.GetAsync<LoanApplicationResponse>($"/api/loanapplication/{id}", token);
        return response.Success ? response.Data : null;
    }

    public async Task<bool> UpdateLoanAsync(Guid id, UpdateLoanRequest request)
    {
        var token = await _authService.GetAccessTokenAsync();
        if (string.IsNullOrEmpty(token))
            return false;

        var response = await _apiService.PutAsync<object>($"/api/loanapplication/{id}", request, token);
        return response.Success;
    }
}

public class SubmitLoanResponse
{
    public Guid Id { get; set; }
}
