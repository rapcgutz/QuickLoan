using QuickLoan.Application.Interfaces;
using QuickLoan.Domain.Enums;

namespace QuickLoan.Infrastructure.Services;

public class QuotationService : IQuotationService
{
    private const decimal EstablishmentFeePercentage = 2.5m;

    public QuotationResult Calculate(decimal amountRequired, int term, ProductType productType)
    {
        var establishmentFee = amountRequired * (EstablishmentFeePercentage / 100);
        var principal = amountRequired + establishmentFee;

        var (annualRate, interestFreeMonths, minTerm) = GetProductSettings(productType);

        // Validate minimum term for ProductB
        if (productType == ProductType.ProductB && term < minTerm)
            throw new InvalidOperationException($"Product B requires minimum {minTerm} months term");

        var monthlyPayment = CalculatePMT(principal, annualRate, term, interestFreeMonths);
        var totalRepayment = monthlyPayment * term;
        var totalInterest = Math.Max(0, totalRepayment - principal);

        return new QuotationResult
        {
            MonthlyRepayment = Math.Round(monthlyPayment, 2),
            EstablishmentFee = Math.Round(establishmentFee, 2),
            TotalInterest = Math.Round(totalInterest, 2),
            TotalRepayment = Math.Round(totalRepayment, 2),
            AnnualInterestRate = annualRate,
            InterestFreeMonths = interestFreeMonths
        };
    }

    private decimal CalculatePMT(decimal principal, decimal annualRate, int months, int interestFreeMonths)
    {
        // If no interest or all months are interest-free, just divide principal by months
        if (annualRate == 0 || months <= interestFreeMonths)
            return principal / months;

        var monthlyRate = (double)(annualRate / 12);
        var effectiveMonths = months - interestFreeMonths;

        // Excel PMT formula: PMT = PV × [r(1+r)^n] / [(1+r)^n - 1]
        var rPlusOne = 1 + monthlyRate;
        var rPlusOnePowerN = Math.Pow(rPlusOne, effectiveMonths);
        
        var numerator = (double)principal * monthlyRate * rPlusOnePowerN;
        var denominator = rPlusOnePowerN - 1;

        // For interest-free period, we need to adjust
        // During interest-free months, customer only pays principal portion
        // After interest-free period, regular PMT kicks in
        
        if (interestFreeMonths > 0)
        {
            // Calculate payment for non-interest-free period
            var paymentAfterFreePeriod = (decimal)(numerator / denominator);
            
            // Total to be paid after free period
            var remainingAfterFreePeriod = paymentAfterFreePeriod * effectiveMonths;
            
            // Distribute evenly across all months
            return remainingAfterFreePeriod / months;
        }

        return (decimal)(numerator / denominator);
    }

    private (decimal AnnualRate, int InterestFreeMonths, int MinTerm) GetProductSettings(ProductType productType)
    {
        return productType switch
        {
            ProductType.ProductA => (0.00m, 0, 1),      // Interest-free
            ProductType.ProductB => (0.08m, 2, 6),      // 8% annual, first 2 months free, min 6 months
            ProductType.ProductC => (0.10m, 0, 1),      // 10% annual
            _ => throw new ArgumentException("Invalid product type")
        };
    }
}
