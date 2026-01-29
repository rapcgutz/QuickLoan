using MediatR;
using QuickLoan.Application.DTOs;
using QuickLoan.Application.Interfaces;
using QuickLoan.Application.Queries;
using QuickLoan.Domain.Entities;
using QuickLoan.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickLoan.Application.Handlers.Queries
{
    public class CalculateQuotationQueryHandler : IRequestHandler<CalculateQuotationQuery, QuotationDto>
    {
        private readonly ILoanApplicationRepository _loanRepo;
        private readonly IQuotationService _quotationService;
        private readonly IApplicationUrlGenerator _urlGenerator;
        private readonly IBlacklistRepository _blacklistRepo;

        public CalculateQuotationQueryHandler(
            ILoanApplicationRepository loanRepo,
            IQuotationService quotationService,
            IApplicationUrlGenerator urlGenerator,
            IBlacklistRepository blacklistRepo)
        {
            _loanRepo = loanRepo;
            _quotationService = quotationService;
            _urlGenerator = urlGenerator;
            _blacklistRepo = blacklistRepo;
        }

        public async Task<QuotationDto> Handle(CalculateQuotationQuery request, CancellationToken cancellationToken)
        {

            // Validate age
            if (!User.IsAtLeast18YearsOld(request.DateOfBirth))
                throw new InvalidOperationException("Applicant must be at least 18 years old");

            // Check blacklists

            var emailDomain = request.Email.Split('@')[1];
            if (await _blacklistRepo.IsMobileBlacklistedAsync(request.Mobile))
                throw new InvalidOperationException("The mobile number you entered cannot be used for this application.");

            if (await _blacklistRepo.IsDomainBlacklistedAsync(emailDomain))
                throw new InvalidOperationException("The Email domain you entered cannot be used for this application.");

            // Validate Product B minimum term
            if (request.ProductType == ProductType.ProductB && request.Term < 6)
                throw new InvalidOperationException("Product B requires minimum 6 months term");

            // Generate application URL
            var applicationUrl = _urlGenerator.Generate(request.FirstName, request.LastName, request.DateOfBirth);

            // Check if application URL already exists
            var existingApplication = await _loanRepo.GetByApplicationUrlAsync(applicationUrl);

            if (existingApplication == null)
            {
                // Create new application
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
                    existingApplication?.UserId);

                // Calculate quotation
                var quotation = _quotationService.Calculate(request.AmountRequired, request.Term, request.ProductType);

                application.SetQuotation(
                    quotation.MonthlyRepayment,
                    quotation.EstablishmentFee,
                    quotation.TotalInterest,
                    quotation.TotalRepayment);

                await _loanRepo.AddAsync(application);

                existingApplication = application;
            }
            else
            {
                // Update existing application with new values
                existingApplication.Update(
                    request.AmountRequired,
                    request.Term,
                    request.ProductType,
                    request.Title,
                    request.Mobile,
                    request.Email);

                var quotation = _quotationService.Calculate(request.AmountRequired, request.Term, request.ProductType);

                existingApplication.SetQuotation(
                    quotation.MonthlyRepayment,
                    quotation.EstablishmentFee,
                    quotation.TotalInterest,
                    quotation.TotalRepayment);

                await _loanRepo.UpdateAsync(existingApplication);
            }

            var result = _quotationService.Calculate(request.AmountRequired, request.Term, request.ProductType);

            return new QuotationDto
            {
                ApplicationUrl = applicationUrl,
                RedirectUrl = $"/loan/{applicationUrl}",
                AmountRequired = request.AmountRequired,
                Term = request.Term,
                ProductType = request.ProductType.ToString(),
                MonthlyRepayment = result.MonthlyRepayment,
                EstablishmentFee = result.EstablishmentFee,
                TotalInterest = result.TotalInterest,
                TotalRepayment = result.TotalRepayment,
                AnnualInterestRate = result.AnnualInterestRate,
                InterestFreeMonths = result.InterestFreeMonths
            };
        }
    }
}
