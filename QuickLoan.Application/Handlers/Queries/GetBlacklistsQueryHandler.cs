using MediatR;
using QuickLoan.Application.DTOs;
using QuickLoan.Application.Interfaces;

namespace QuickLoan.Application.Queries;
public class GetBlacklistsQueryHandler : IRequestHandler<GetBlacklistsQuery, BlacklistsDto>
{
    private readonly IBlacklistRepository _blacklistRepo;

    public GetBlacklistsQueryHandler(IBlacklistRepository blacklistRepo)
    {
        _blacklistRepo = blacklistRepo;
    }

    public async Task<BlacklistsDto> Handle(GetBlacklistsQuery request, CancellationToken cancellationToken)
    {
        var mobiles = await _blacklistRepo.GetBlacklistedMobilesAsync();
        var domains = await _blacklistRepo.GetBlacklistedDomainsAsync();

        return new BlacklistsDto
        {
            BlockedMobiles = mobiles,
            BlockedDomains = domains
        };
    }
}
