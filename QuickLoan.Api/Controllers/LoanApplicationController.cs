using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuickLoan.Api.Models;
using QuickLoan.Application.Commands;
using QuickLoan.Application.Queries;
using QuickLoan.Domain.Entities;
using QuickLoan.Domain.Enums;
using System.Security.Claims;

namespace QuickLoan.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LoanApplicationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LoanApplicationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitLoanApplication([FromBody] SubmitLoanRequest request)
        {
            var userId = GetCurrentUserId();

            var command = new CreateLoanApplicationCommand
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

            var applicationId = await _mediator.Send(command);
            return Ok(new ApiResponse<object>(new { id = applicationId }));
        }

        [HttpGet("my-loans")]
        public async Task<IActionResult> GetMyLoans()
        {
            var userId = GetCurrentUserId();
            var query = new GetUserLoansQuery { UserId = userId };
            var result = await _mediator.Send(query);
            return Ok(new ApiResponse<object>(result));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLoanApplication(Guid id)
        {
            var userId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin");

            var query = new GetLoanApplicationByIdQuery
            {
                Id = id,
                RequestingUserId = userId,
                IsAdmin = isAdmin
            };

            var result = await _mediator.Send(query);
            return Ok(new ApiResponse<object>(result));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLoanApplication(Guid id, [FromBody] UpdateLoanRequest request)
        {
            var userId = GetCurrentUserId();

            var command = new UpdateLoanApplicationCommand
            {
                Id = id,
                UserId = userId,
                AmountRequired = request.AmountRequired,
                Term = request.Term,
                ProductType = Enum.Parse<ProductType>(request.ProductType)
            };

            await _mediator.Send(command);
            return NoContent();
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedAccessException("Invalid user ID");

            return userId;
        }
    }

    public record SubmitLoanRequest(decimal AmountRequired, int Term, string ProductType, string Title,
        string FirstName, string LastName, DateTime DateOfBirth, string Mobile, string Email);
    public record UpdateLoanRequest(decimal AmountRequired, int Term, string ProductType);
}
