using MediatR;
using QuickLoan.Application.DTOs;
using QuickLoan.Application.Interfaces;
using QuickLoan.Domain.Entities;

namespace QuickLoan.Application.Commands;

public class LoginCommand : IRequest<TokenDto>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
