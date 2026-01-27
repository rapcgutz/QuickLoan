using MediatR;
using QuickLoan.Application.DTOs;
using QuickLoan.Application.Interfaces;
using QuickLoan.Domain.Entities;
using QuickLoan.Domain.Enums;

namespace QuickLoan.Application.Queries;

public class CalculateQuotationQuery : IRequest<QuotationDto>
{
    public decimal AmountRequired { get; set; }
    public int Term { get; set; }
    public ProductType ProductType { get; set; }
    public Title Title { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Mobile { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
