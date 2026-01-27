using QuickLoan.Domain.Entities;

namespace QuickLoan.Application.Interfaces;

public interface IBlacklistRepository
{
    Task<bool> IsMobileBlacklistedAsync(string mobile);
    Task<bool> IsDomainBlacklistedAsync(string domain);
    Task<List<string>> GetBlacklistedMobilesAsync();
    Task<List<string>> GetBlacklistedDomainsAsync();
    Task AddBlacklistedMobileAsync(BlacklistedMobile mobile);
    Task AddBlacklistedDomainAsync(BlacklistedDomain domain);
    Task RemoveBlacklistedMobileAsync(string mobile);
    Task RemoveBlacklistedDomainAsync(string domain);
}
