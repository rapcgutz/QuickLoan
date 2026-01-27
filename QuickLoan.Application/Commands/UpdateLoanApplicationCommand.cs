using MediatR;
using QuickLoan.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuickLoan.Application.Commands
{
    public class UpdateLoanApplicationCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public decimal AmountRequired { get; set; }
        public int Term { get; set; }
        public ProductType ProductType { get; set; }
    }

}
