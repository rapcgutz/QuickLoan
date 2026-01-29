using MediatR;
using QuickLoan.Application.DTOs;
using QuickLoan.Application.Interfaces;
using QuickLoan.Application.Queries;
using QuickLoan.Domain.Enums;

namespace QuickLoan.Application.Handlers.Queries
{
    public class GetAllLoansQueryHandler : IRequestHandler<GetAllLoansQuery, List<LoanApplicationDto>>
    {
        private readonly ILoanApplicationRepository _loanRepo;

        public GetAllLoansQueryHandler(ILoanApplicationRepository loanRepo)
        {
            _loanRepo = loanRepo;
        }

        public async Task<List<LoanApplicationDto>> Handle(
            GetAllLoansQuery request,
            CancellationToken cancellationToken)
        {
            // Get all loans with optional filters
            var loans = await _loanRepo.GetAllAsync(
                request.Status,
                request.Page ?? 1,
                request.PageSize ?? 50);

            // Map to DTOs
            return loans.Select(loan => new LoanApplicationDto
            {
                Id = loan.Id,
                UserId = loan.UserId,
                Title = loan.Title.ToString(),
                FirstName = loan.FirstName,
                LastName = loan.LastName,
                Email = loan.Email,
                Mobile = loan.Mobile,
                DateOfBirth = loan.DateOfBirth,
                AmountRequired = loan.AmountRequired,
                Term = loan.Term,
                ProductType = loan.ProductType.ToString(),
                Status = loan.Status.ToString(),
                MonthlyRepayment = loan.MonthlyRepayment,
                EstablishmentFee = loan.EstablishmentFee,
                TotalInterest = loan.TotalInterest,
                TotalRepayment = loan.TotalRepayment,
                CreatedAt = loan.CreatedAt,
                UpdatedAt = loan.UpdatedAt,
                SubmittedAt = loan.SubmittedAt,
                ApplicationUrl = loan.ApplicationUrl,
                AdminNotes = loan.AdminNotes
            }).ToList();
        }
    }
}