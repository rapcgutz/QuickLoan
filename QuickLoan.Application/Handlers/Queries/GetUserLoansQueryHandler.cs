using MediatR;
using QuickLoan.Application.DTOs;
using QuickLoan.Application.Interfaces;
using QuickLoan.Application.Queries;
using QuickLoan.Domain.Entities;

namespace QuickLoan.Application.Handlers.Queries;

public class GetUserLoansQueryHandler : IRequestHandler<GetUserLoansQuery, List<LoanApplicationDto>>
{
    private readonly ILoanApplicationRepository _loanRepo;

    public GetUserLoansQueryHandler(ILoanApplicationRepository loanRepo)
    {
        _loanRepo = loanRepo;
    }

    public async Task<List<LoanApplicationDto>> Handle(GetUserLoansQuery request, CancellationToken cancellationToken)
    {
        var applications = await _loanRepo.GetUserLoansAsync(request.UserId);
        
        return applications.Select(MapToDto).ToList();
    }

    private static LoanApplicationDto MapToDto(LoanApplication app)
    {
        return new LoanApplicationDto
        {
            Id = app.Id,
            UserId = app.UserId,
            ApplicationUrl = app.ApplicationUrl,
            Title = app.Title.ToString(),
            FirstName = app.FirstName,
            LastName = app.LastName,
            DateOfBirth = app.DateOfBirth,
            Mobile = app.Mobile,
            Email = app.Email,
            AmountRequired = app.AmountRequired,
            Term = app.Term,
            ProductType = app.ProductType.ToString(),
            MonthlyRepayment = app.MonthlyRepayment,
            EstablishmentFee = app.EstablishmentFee,
            TotalInterest = app.TotalInterest,
            TotalRepayment = app.TotalRepayment,
            Status = app.Status.ToString(),
            CreatedAt = app.CreatedAt,
            UpdatedAt = app.UpdatedAt,
            SubmittedAt = app.SubmittedAt,
            ProcessedAt = app.ProcessedAt,
            AdminNotes = app.AdminNotes
        };
    }
}
