using MediatR;
using QuickLoan.Application.Commands;
using QuickLoan.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuickLoan.Application.Handlers.Commands
{
    public class UpdateLoanApplicationCommandHandler : IRequestHandler<UpdateLoanApplicationCommand, Unit>
    {
        private readonly ILoanApplicationRepository _loanRepo;
        private readonly IQuotationService _quotationService;

        public UpdateLoanApplicationCommandHandler(
            ILoanApplicationRepository loanRepo,
            IQuotationService quotationService)
        {
            _loanRepo = loanRepo;
            _quotationService = quotationService;
        }

        public async Task<Unit> Handle(UpdateLoanApplicationCommand request, CancellationToken cancellationToken)
        {
            var application = await _loanRepo.GetByIdAsync(request.Id);

            if (application == null)
                throw new InvalidOperationException("Loan application not found");

            if (application.UserId != request.UserId)
                throw new UnauthorizedAccessException("Not authorized to update this application");

            if (!application.CanBeEdited())
                throw new InvalidOperationException("Cannot edit submitted applications");

            application.Update(request.AmountRequired, request.Term, request.ProductType);

            var quotation = _quotationService.Calculate(request.AmountRequired, request.Term, request.ProductType);
            application.SetQuotation(
                quotation.MonthlyRepayment,
                quotation.EstablishmentFee,
                quotation.TotalInterest,
                quotation.TotalRepayment);

            await _loanRepo.UpdateAsync(application);

            return Unit.Value;
        }
    }
}
