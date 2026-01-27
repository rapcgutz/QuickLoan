using MediatR;
using QuickLoan.Application.Commands;
using QuickLoan.Application.Interfaces;

namespace QuickLoan.Application.Handlers.Commands
{
    public class UnblockDomainCommandHandler : IRequestHandler<UnblockDomainCommand, Unit>
    {
        private readonly IBlacklistRepository _blacklistRepo;

        public UnblockDomainCommandHandler(IBlacklistRepository blacklistRepo)
        {
            _blacklistRepo = blacklistRepo;
        }

        public async Task<Unit> Handle(UnblockDomainCommand request, CancellationToken cancellationToken)
        {
            await _blacklistRepo.RemoveBlacklistedDomainAsync(request.Domain);
            return Unit.Value;
        }
    }
}
