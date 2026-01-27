using QuickLoan.Domain.Enums;

namespace QuickLoan.Application.DTOs;

public class LoanApplicationDto
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public string ApplicationUrl { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Mobile { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal AmountRequired { get; set; }
    public int Term { get; set; }
    public string ProductType { get; set; } = string.Empty;
    public decimal MonthlyRepayment { get; set; }
    public decimal EstablishmentFee { get; set; }
    public decimal TotalInterest { get; set; }
    public decimal TotalRepayment { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public string? AdminNotes { get; set; }
}
