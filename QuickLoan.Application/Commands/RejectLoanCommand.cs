using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuickLoan.Application.Commands
{
    public class RejectLoanCommand : IRequest<Unit>
    {
        public Guid LoanApplicationId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
