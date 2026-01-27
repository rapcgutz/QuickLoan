using MediatR;
using QuickLoan.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickLoan.Application.Queries
{
    public class GetBlacklistsQuery : IRequest<BlacklistsDto>
    {
    }
}
