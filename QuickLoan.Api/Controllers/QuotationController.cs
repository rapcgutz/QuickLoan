using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuickLoan.Api.Models;
using QuickLoan.Application.Queries;
using QuickLoan.Domain.Enums;

namespace QuickLoan.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuotationController : ControllerBase
{
    private readonly IMediator _mediator;

    public QuotationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("calculate")]
    public async Task<IActionResult> CalculateQuotation([FromBody] CalculateQuotationRequest request)
    {
        var command = new CalculateQuotationQuery
        {
            AmountRequired = request.AmountRequired,
            Term = request.Term,
            ProductType = Enum.Parse<ProductType>(request.ProductType),
            Title = Enum.Parse<Title>(request.Title),
            FirstName = request.FirstName,
            LastName = request.LastName,
            DateOfBirth = request.DateOfBirth,
            Mobile = request.Mobile,
            Email = request.Email
        };

        var result = await _mediator.Send(command);
        return Ok(new ApiResponse<object>(result));
    }

    [HttpGet("loan/{applicationUrl}")]
    public async Task<IActionResult> GetByUrl(string applicationUrl)
    {
        var query = new GetLoanApplicationByUrlQuery { ApplicationUrl = applicationUrl };
        var result = await _mediator.Send(query);
        return Ok(new ApiResponse<object>(result));
    }
}

public record CalculateQuotationRequest(
    decimal AmountRequired,
    int Term,
    string ProductType,
    string Title,
    string FirstName,
    string LastName,
    DateTime DateOfBirth,
    string Mobile,
    string Email);