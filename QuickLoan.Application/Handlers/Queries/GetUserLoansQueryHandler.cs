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

        private LoanApplicationDto MapToDto(LoanApplication app)
        {
        }
    }
}
