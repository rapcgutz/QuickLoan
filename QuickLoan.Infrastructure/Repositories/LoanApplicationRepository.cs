using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<LoanApplicationRepository> _logger;

        public LoanApplicationRepository(QuickLoanDbContext context, ILogger<LoanApplicationRepository> logger)
        {
            _context = context;
            _logger = logger;
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
            try
            {
                _logger.LogInformation("Adding loan: UserId={UserId}, Amount={Amount}, Status={Status}", application.UserId, application.AmountRequired, application.Status);

                if (application.UserId == Guid.Empty)
                {
                    _logger.LogError("UserId is EMPTY!");
                    throw new InvalidOperationException("UserId cannot be empty");
                }

                await _context.LoanApplications.AddAsync(application);

                var result = await _context.SaveChangesAsync();

                _logger.LogInformation("SaveChanges result: {Result} rows affected", result);

                if (result == 0)
                {
                    _logger.LogError("NO ROWS SAVED!");
                }

                return application.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception: on AddAsync() - {ex.InnerException}");
            }
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
