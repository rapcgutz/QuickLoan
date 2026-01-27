using MediatR;
using QuickLoan.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuickLoan.Application.Commands
{
    public class RefreshTokenCommand : IRequest<TokenDto>
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
