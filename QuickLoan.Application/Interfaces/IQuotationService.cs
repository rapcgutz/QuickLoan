using QuickLoan.Domain.Enums;

namespace QuickLoan.Application.Interfaces;

public interface IQuotationService
{
    QuotationResult Calculate(decimal amountRequired, int term, ProductType productType);
}

public class QuotationResult
{
    public decimal MonthlyRepayment { get; set; }
    public decimal EstablishmentFee { get; set; }
    public decimal TotalInterest { get; set; }
    public decimal TotalRepayment { get; set; }
    public decimal AnnualInterestRate { get; set; }
    public int InterestFreeMonths { get; set; }
}
