using MediatR;
using QuickLoan.Application.Commands;
using QuickLoan.Application.Interfaces;
using QuickLoan.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuickLoan.Application.Handlers.Commands
{
    public class BlockDomainCommandHandler : IRequestHandler<BlockDomainCommand, Unit>
    {
        private readonly IBlacklistRepository _blacklistRepo;

        public BlockDomainCommandHandler(IBlacklistRepository blacklistRepo)
        {
            _blacklistRepo = blacklistRepo;
        }

        public async Task<Unit> Handle(BlockDomainCommand request, CancellationToken cancellationToken)
        {
            if (await _blacklistRepo.IsMobileBlacklistedAsync(request.Domain))
                throw new InvalidOperationException("Mobile is already blacklisted");

            var blacklistedDomain = BlacklistedDomain.Create(request.Domain, request.AdminUserId, request.Reason);
            await _blacklistRepo.AddBlacklistedDomainAsync(blacklistedDomain);

            return Unit.Value;
        }
    }
}
