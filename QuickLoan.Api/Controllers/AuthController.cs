using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuickLoan.Api.Models;
using QuickLoan.Application.Commands;

namespace QuickLoan.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var command = new RegisterUserCommand
            {
                Email = request.Email,
                Password = request.Password,
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth,
                Mobile = request.Mobile
            };

            var result = await _mediator.Send(command);
            return Ok(new ApiResponse<object>(result));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var command = new LoginCommand
            {
                Email = request.Email,
                Password = request.Password
            };

            var result = await _mediator.Send(command);
            return Ok(new ApiResponse<object>(result));
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var command = new RefreshTokenCommand
            {
                RefreshToken = request.RefreshToken
            };

            var result = await _mediator.Send(command);
            return Ok(new ApiResponse<object>(result));
        }
    }

    public record RegisterRequest(string Email, string Password, string FirstName, string LastName, DateTime DateOfBirth, string Mobile);
    public record LoginRequest(string Email, string Password);
    public record RefreshTokenRequest(string RefreshToken);
}
