using MediatR;
using QuickLoan.Application.DTOs;
using QuickLoan.Application.Interfaces;
using QuickLoan.Application.Queries;
using QuickLoan.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuickLoan.Application.Handlers.Queries
{
    public class GetLoanApplicationByUrlQueryHandler : IRequestHandler<GetLoanApplicationByUrlQuery, LoanApplicationDto>
    {
        private readonly ILoanApplicationRepository _loanRepo;

        public GetLoanApplicationByUrlQueryHandler(ILoanApplicationRepository loanRepo)
        {
            _loanRepo = loanRepo;
        }

        public async Task<LoanApplicationDto> Handle(GetLoanApplicationByUrlQuery request, CancellationToken cancellationToken)
        {
            var application = await _loanRepo.GetByApplicationUrlAsync(request.ApplicationUrl);

            if (application == null)
                throw new InvalidOperationException("Loan application not found");

            return MapToDto(application);
        }

        private LoanApplicationDto MapToDto(LoanApplication app)
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
}
