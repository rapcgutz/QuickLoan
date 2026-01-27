using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickLoan.Api.Models;
using QuickLoan.Application.Commands;
using QuickLoan.Application.Handlers.Queries;
using QuickLoan.Domain.Entities;
using QuickLoan.Domain.Enums;
using System.Security.Claims;

namespace QuickLoan.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("loans")]
        public async Task<IActionResult> GetAllLoans([FromQuery] string? status, [FromQuery] int? page, [FromQuery] int? pageSize)
        {
            var query = new GetAllLoansQuery
            {
                Status = string.IsNullOrEmpty(status) ? null : Enum.Parse<ApplicationStatus>(status),
                Page = page,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);
            return Ok(new ApiResponse<object>(result));
        }

        [HttpPost("loans/{id}/approve")]
        public async Task<IActionResult> ApproveLoan(Guid id, [FromBody] ApproveRequest request)
        {
            var command = new ApproveLoanCommand
            {
                LoanApplicationId = id,
                Notes = request.Notes
            };

            await _mediator.Send(command);
            return NoContent();
        }

        [HttpPost("loans/{id}/reject")]
        public async Task<IActionResult> RejectLoan(Guid id, [FromBody] RejectRequest request)
        {
            var command = new RejectLoanCommand
            {
                LoanApplicationId = id,
                Reason = request.Reason
            };

            await _mediator.Send(command);
            return NoContent();
        }

        [HttpPost("blacklist/mobile")]
        public async Task<IActionResult> BlockMobile([FromBody] BlockMobileRequest request)
        {
            var userId = GetCurrentUserId();

            var command = new BlockMobileCommand
            {
                Mobile = request.Mobile,
                AdminUserId = userId,
                Reason = request.Reason
            };

            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("blacklist/mobile/{mobile}")]
        public async Task<IActionResult> UnblockMobile(string mobile)
        {
            var command = new UnblockMobileCommand { Mobile = mobile };
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpPost("blacklist/domain")]
        public async Task<IActionResult> BlockDomain([FromBody] BlockDomainRequest request)
        {
            var userId = GetCurrentUserId();

            var command = new BlockDomainCommand
            {
                Domain = request.Domain,
                AdminUserId = userId,
                Reason = request.Reason
            };

            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("blacklist/domain/{domain}")]
        public async Task<IActionResult> UnblockDomain(string domain)
        {
            var command = new UnblockDomainCommand { Domain = domain };
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpGet("blacklist")]
        public async Task<IActionResult> GetBlacklists()
        {
            var query = new GetBlacklistsQuery();
            var result = await _mediator.Send(query);
            return Ok(new ApiResponse<object>(result));
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedAccessException("Invalid user ID");

            return userId;
        }
    }

    public record ApproveRequest(string? Notes);
    public record RejectRequest(string Reason);
    public record BlockMobileRequest(string Mobile, string? Reason);
    public record BlockDomainRequest(string Domain, string? Reason);
}
