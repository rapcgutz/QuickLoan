using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuickLoan.Application.Commands
{
    public class BlockMobileCommand : IRequest<Unit>
    {
        public string Mobile { get; set; } = string.Empty;
        public Guid AdminUserId { get; set; }
        public string? Reason { get; set; }
    }
}
