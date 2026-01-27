using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuickLoan.Application.Commands
{
    public class ApproveLoanCommand : IRequest<Unit>
    {
        public Guid LoanApplicationId { get; set; }
        public string? Notes { get; set; }
    }
}
