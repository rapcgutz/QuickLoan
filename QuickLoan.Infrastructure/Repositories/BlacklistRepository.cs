using Microsoft.EntityFrameworkCore;
using QuickLoan.Application.Interfaces;
using QuickLoan.Domain.Entities;
using QuickLoan.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuickLoan.Infrastructure.Repositories
{
    public class BlacklistRepository : IBlacklistRepository
    {
        private readonly QuickLoanDbContext _context;

        public BlacklistRepository(QuickLoanDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsMobileBlacklistedAsync(string mobile)
        {
            return await _context.BlacklistedMobiles.AnyAsync(bm => bm.Mobile == mobile);
        }

        public async Task<bool> IsDomainBlacklistedAsync(string domain)
        {
            return await _context.BlacklistedDomains.AnyAsync(bd => bd.Domain == domain.ToLower());
        }

        public async Task<List<string>> GetBlacklistedMobilesAsync()
        {
            return await _context.BlacklistedMobiles.Select(bm => bm.Mobile).ToListAsync();
        }

        public async Task<List<string>> GetBlacklistedDomainsAsync()
        {
            return await _context.BlacklistedDomains.Select(bd => bd.Domain).ToListAsync();
        }

        public async Task AddBlacklistedMobileAsync(BlacklistedMobile mobile)
        {
            await _context.BlacklistedMobiles.AddAsync(mobile);
            await _context.SaveChangesAsync();
        }

        public async Task AddBlacklistedDomainAsync(BlacklistedDomain domain)
        {
            await _context.BlacklistedDomains.AddAsync(domain);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveBlacklistedMobileAsync(string mobile)
        {
            var entity = await _context.BlacklistedMobiles.FirstOrDefaultAsync(bm => bm.Mobile == mobile);
            if (entity != null)
            {
                _context.BlacklistedMobiles.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveBlacklistedDomainAsync(string domain)
        {
            var entity = await _context.BlacklistedDomains.FirstOrDefaultAsync(bd => bd.Domain == domain.ToLower());
            if (entity != null)
            {
                _context.BlacklistedDomains.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
