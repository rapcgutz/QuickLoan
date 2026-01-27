using MediatR;
using QuickLoan.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuickLoan.Application.Queries
{
    public class GetLoanApplicationByUrlQuery : IRequest<LoanApplicationDto>
    {
        public string ApplicationUrl { get; set; } = string.Empty;
    }
}
