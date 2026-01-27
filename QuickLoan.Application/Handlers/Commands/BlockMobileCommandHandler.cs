using MediatR;
using QuickLoan.Application.Commands;
using QuickLoan.Application.Interfaces;
using QuickLoan.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuickLoan.Application.Handlers.Commands
{
    public class BlockMobileCommandHandler : IRequestHandler<BlockMobileCommand, Unit>
    {
        private readonly IBlacklistRepository _blacklistRepo;

        public BlockMobileCommandHandler(IBlacklistRepository blacklistRepo)
        {
            _blacklistRepo = blacklistRepo;
        }

        public async Task<Unit> Handle(BlockMobileCommand request, CancellationToken cancellationToken)
        {
            if (await _blacklistRepo.IsMobileBlacklistedAsync(request.Mobile))
                throw new InvalidOperationException("Mobile is already blacklisted");

            var blacklistedMobile = BlacklistedMobile.Create(request.Mobile, request.AdminUserId, request.Reason);
            await _blacklistRepo.AddBlacklistedMobileAsync(blacklistedMobile);

            return Unit.Value;
        }
    }
}
