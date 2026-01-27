using Microsoft.EntityFrameworkCore;
using QuickLoan.Application.Interfaces;
using QuickLoan.Domain.Entities;
using QuickLoan.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuickLoan.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly QuickLoanDbContext _context;

        public UserRepository(QuickLoanDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email.ToLower());
        }

        public async Task<Guid> AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user.Id;
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email.ToLower());
        }
    }
}
