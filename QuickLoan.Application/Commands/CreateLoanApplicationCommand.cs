using MediatR;
using QuickLoan.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuickLoan.Application.Commands
{
    public class CreateLoanApplicationCommand : IRequest<Guid>
    {
        public decimal AmountRequired { get; set; }
        public int Term { get; set; }
        public ProductType ProductType { get; set; }
        public Title Title { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Mobile { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
