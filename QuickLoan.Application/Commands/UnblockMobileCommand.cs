using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuickLoan.Application.Commands
{
    public class UnblockMobileCommand : IRequest<Unit>
    {
        public string Mobile { get; set; } = string.Empty;
    }
}
