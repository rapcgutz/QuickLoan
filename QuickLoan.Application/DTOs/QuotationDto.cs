namespace QuickLoan.Application.DTOs;

public class QuotationDto
{
    public string ApplicationUrl { get; set; } = string.Empty;
    public string RedirectUrl { get; set; } = string.Empty;
    public decimal AmountRequired { get; set; }
    public int Term { get; set; }
    public string ProductType { get; set; } = string.Empty;
    public decimal MonthlyRepayment { get; set; }
    public decimal EstablishmentFee { get; set; }
    public decimal TotalInterest { get; set; }
    public decimal TotalRepayment { get; set; }
    public decimal AnnualInterestRate { get; set; }
    public int InterestFreeMonths { get; set; }
}
