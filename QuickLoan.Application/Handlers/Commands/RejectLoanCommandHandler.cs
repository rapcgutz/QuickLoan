using MediatR;
using QuickLoan.Application.Commands;
using QuickLoan.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuickLoan.Application.Handlers.Commands
{
    public class RejectLoanCommandHandler : IRequestHandler<RejectLoanCommand, Unit>
    {
        private readonly ILoanApplicationRepository _loanRepo;

        public RejectLoanCommandHandler(ILoanApplicationRepository loanRepo)
        {
            _loanRepo = loanRepo;
        }

        public async Task<Unit> Handle(RejectLoanCommand request, CancellationToken cancellationToken)
        {
            var application = await _loanRepo.GetByIdAsync(request.LoanApplicationId);

            if (application == null)
                throw new InvalidOperationException("Loan application not found");

            application.Reject(request.Reason);
            await _loanRepo.UpdateAsync(application);

            return Unit.Value;
        }
    }
}
