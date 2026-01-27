using QuickLoan.Domain.Entities;
using QuickLoan.Domain.Enums;

namespace QuickLoan.Application.Interfaces;

public interface ILoanApplicationRepository
{
    Task<LoanApplication?> GetByIdAsync(Guid id);
    Task<LoanApplication?> GetByApplicationUrlAsync(string applicationUrl);
    Task<List<LoanApplication>> GetUserLoansAsync(Guid userId);
    Task<List<LoanApplication>> GetAllAsync(ApplicationStatus? status = null, int? page = null, int? pageSize = null);
    Task<Guid> AddAsync(LoanApplication application);
    Task UpdateAsync(LoanApplication application);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> ApplicationUrlExistsAsync(string applicationUrl);
}
