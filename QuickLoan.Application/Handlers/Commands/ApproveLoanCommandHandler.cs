using MediatR;
using QuickLoan.Application.Commands;
using QuickLoan.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuickLoan.Application.Handlers.Commands
{
    public class ApproveLoanCommandHandler : IRequestHandler<ApproveLoanCommand, Unit>
    {
        private readonly ILoanApplicationRepository _loanRepo;

        public ApproveLoanCommandHandler(ILoanApplicationRepository loanRepo)
        {
            _loanRepo = loanRepo;
        }

        public async Task<Unit> Handle(ApproveLoanCommand request, CancellationToken cancellationToken)
        {
            var application = await _loanRepo.GetByIdAsync(request.LoanApplicationId);

            if (application == null)
                throw new InvalidOperationException("Loan application not found");

            application.Approve(request.Notes);
            await _loanRepo.UpdateAsync(application);

            return Unit.Value;
        }
    }
}
