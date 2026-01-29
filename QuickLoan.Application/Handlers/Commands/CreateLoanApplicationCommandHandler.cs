using MediatR;
using QuickLoan.Application.Commands;
using QuickLoan.Application.Interfaces;
using QuickLoan.Domain.Entities;
using QuickLoan.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuickLoan.Application.Handlers.Commands
{
    public class CreateLoanApplicationCommandHandler : IRequestHandler<CreateLoanApplicationCommand, Guid>
    {
        private readonly ILoanApplicationRepository _loanRepo;
        private readonly IQuotationService _quotationService;
        private readonly IApplicationUrlGenerator _urlGenerator;
        private readonly IBlacklistRepository _blacklistRepo;
        private readonly ICurrentUserService _currentUserService;
        public CreateLoanApplicationCommandHandler(
            ILoanApplicationRepository loanRepo,
            IQuotationService quotationService,
            IApplicationUrlGenerator urlGenerator,
            IBlacklistRepository blacklistRepo,
            ICurrentUserService currentUserService)
        {
            _loanRepo = loanRepo;
            _quotationService = quotationService;
            _urlGenerator = urlGenerator;
            _blacklistRepo = blacklistRepo;
            _currentUserService = currentUserService;
        }

        public async Task<Guid> Handle(CreateLoanApplicationCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            if (userId == Guid.Empty)
            {
                throw new UnauthorizedAccessException("User not authenticated");
            }

            // Validations
            if (!User.IsAtLeast18YearsOld(request.DateOfBirth))
                throw new InvalidOperationException("Applicant must be at least 18 years old");


            var emailDomain = request.Email.Split('@')[1];
            if (await _blacklistRepo.IsMobileBlacklistedAsync(request.Mobile))
                throw new InvalidOperationException("The mobile number you entered cannot be used for this application.");

            if (await _blacklistRepo.IsDomainBlacklistedAsync(emailDomain))
                throw new InvalidOperationException("The Email domain you entered cannot be used for this application.");

            if (request.ProductType == ProductType.ProductB && request.Term < 6)
                throw new InvalidOperationException("Product B requires minimum 6 months term");

            // Generate URL
            var applicationUrl = _urlGenerator.Generate(request.FirstName, request.LastName, request.DateOfBirth);

            // Create application
            var application = LoanApplication.Create(
                request.Title,
                request.FirstName,
                request.LastName,
                request.DateOfBirth,
                request.Mobile,
                request.Email,
                request.AmountRequired,
                request.Term,
                request.ProductType,
                applicationUrl,
                userId);

            // Calculate and set quotation
            var quotation = _quotationService.Calculate(request.AmountRequired, request.Term, request.ProductType);
            application.SetQuotation(
                quotation.MonthlyRepayment,
                quotation.EstablishmentFee,
                quotation.TotalInterest,
                quotation.TotalRepayment);

            // Submit immediately
            application.Submit(userId);

            return await _loanRepo.AddAsync(application);
        }
    }
}
