using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuickLoan.Application.Commands
{
    public class UnblockDomainCommand : IRequest<Unit>
    {
        public string Domain { get; set; } = string.Empty;
    }
}
