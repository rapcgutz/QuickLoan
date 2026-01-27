using MediatR;
using QuickLoan.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuickLoan.Application.Queries
{
    public class GetLoanApplicationByIdQuery : IRequest<LoanApplicationDto>
    {
        public Guid Id { get; set; }
        public Guid RequestingUserId { get; set; }
        public bool IsAdmin { get; set; }
    }
}
