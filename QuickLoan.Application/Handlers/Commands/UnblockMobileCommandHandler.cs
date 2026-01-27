using MediatR;
using QuickLoan.Application.Commands;
using QuickLoan.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuickLoan.Application.Handlers.Commands
{
    public class UnblockMobileCommandHandler : IRequestHandler<UnblockMobileCommand, Unit>
    {
        private readonly IBlacklistRepository _blacklistRepo;

        public UnblockMobileCommandHandler(IBlacklistRepository blacklistRepo)
        {
            _blacklistRepo = blacklistRepo;
        }

        public async Task<Unit> Handle(UnblockMobileCommand request, CancellationToken cancellationToken)
        {
            await _blacklistRepo.RemoveBlacklistedMobileAsync(request.Mobile);
            return Unit.Value;
        }
    }
}
