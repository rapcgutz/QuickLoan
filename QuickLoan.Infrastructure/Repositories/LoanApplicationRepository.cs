using Microsoft.EntityFrameworkCore;
using QuickLoan.Application.Interfaces;
using QuickLoan.Domain.Entities;
using QuickLoan.Domain.Enums;
using QuickLoan.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuickLoan.Infrastructure.Repositories
{
    public class LoanApplicationRepository : ILoanApplicationRepository
    {
        private readonly QuickLoanDbContext _context;

        public LoanApplicationRepository(QuickLoanDbContext context)
        {
            _context = context;
        }

        public async Task<LoanApplication?> GetByIdAsync(Guid id)
        {
            return await _context.LoanApplications
                .Include(la => la.User)
                .FirstOrDefaultAsync(la => la.Id == id);
        }

        public async Task<LoanApplication?> GetByApplicationUrlAsync(string applicationUrl)
        {
            return await _context.LoanApplications
                .Include(la => la.User)
                .FirstOrDefaultAsync(la => la.ApplicationUrl == applicationUrl);
        }

        public async Task<List<LoanApplication>> GetUserLoansAsync(Guid userId)
        {
            return await _context.LoanApplications
                .Where(la => la.UserId == userId)
                .OrderByDescending(la => la.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<LoanApplication>> GetAllAsync(ApplicationStatus? status = null, int? page = null, int? pageSize = null)
        {
            var query = _context.LoanApplications
                .Include(la => la.User)
                .AsQueryable();

            if (status.HasValue)
                query = query.Where(la => la.Status == status.Value);

            query = query.OrderByDescending(la => la.CreatedAt);

            if (page.HasValue && pageSize.HasValue)
                query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);

            return await query.ToListAsync();
        }

        public async Task<Guid> AddAsync(LoanApplication application)
        {
            await _context.LoanApplications.AddAsync(application);
            await _context.SaveChangesAsync();
            return application.Id;
        }

        public async Task UpdateAsync(LoanApplication application)
        {
            _context.LoanApplications.Update(application);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.LoanApplications.AnyAsync(la => la.Id == id);
        }

        public async Task<bool> ApplicationUrlExistsAsync(string applicationUrl)
        {
            return await _context.LoanApplications.AnyAsync(la => la.ApplicationUrl == applicationUrl);
        }
    }
}
