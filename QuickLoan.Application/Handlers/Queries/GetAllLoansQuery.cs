using MediatR;
using QuickLoan.Application.DTOs;
using QuickLoan.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuickLoan.Application.Handlers.Queries
{
    public class GetAllLoansQuery : IRequest<List<LoanApplicationDto>>
    {
        public ApplicationStatus? Status { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }
}
