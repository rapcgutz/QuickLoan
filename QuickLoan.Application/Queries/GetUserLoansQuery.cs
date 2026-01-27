using MediatR;
using QuickLoan.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuickLoan.Application.Queries
{
    public class GetUserLoansQuery : IRequest<List<LoanApplicationDto>>
    {
        public Guid UserId { get; set; }
    }
}
