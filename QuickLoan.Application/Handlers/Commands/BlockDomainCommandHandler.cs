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
            if(!IsValidDomain(request.Domain))
                throw new InvalidOperationException("Invalid Domain.");

            if (await _blacklistRepo.IsMobileBlacklistedAsync(request.Domain))
                throw new InvalidOperationException("Mobile is already blacklisted");

            var blacklistedDomain = BlacklistedDomain.Create(request.Domain, request.AdminUserId, request.Reason);
            await _blacklistRepo.AddBlacklistedDomainAsync(blacklistedDomain);

            return Unit.Value;
        }

        public static bool IsValidDomain(string domain)
        {
            if (string.IsNullOrWhiteSpace(domain))
                return false;

            domain = domain.Trim().ToLowerInvariant();

            // Must NOT contain @
            if (domain.Contains("@"))
                return false;

            // Must contain at least one dot
            if (!domain.Contains('.'))
                return false;

            // No spaces
            if (domain.Any(char.IsWhiteSpace))
                return false;

            try
            {
                // Validates DNS-safe format
                var uri = new Uri($"http://{domain}");
                return uri.Host == domain;
            }
            catch
            {
                return false;
            }
        }
    }
}
