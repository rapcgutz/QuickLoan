using QuickLoan.Web.Models;

namespace QuickLoan.Web.Interfaces;

public interface ILoanService
{
    Task<ApiResponse<QuotationResponse?>> CalculateQuotationAsync(QuotationRequest request);
    Task<LoanApplicationResponse?> GetLoanByUrlAsync(string applicationUrl);
    Task<Guid?> SubmitLoanApplicationAsync(LoanApplicationRequest request);
    Task<List<LoanApplicationResponse>?> GetMyLoansAsync();
    Task<LoanApplicationResponse?> GetLoanByIdAsync(Guid id);
    Task<bool> UpdateLoanAsync(Guid id, UpdateLoanRequest request);
}
