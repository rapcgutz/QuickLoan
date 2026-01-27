using Microsoft.EntityFrameworkCore;
using QuickLoan.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuickLoan.Infrastructure.Data
{
    public class QuickLoanDbContext : DbContext
    {
        public QuickLoanDbContext(DbContextOptions<QuickLoanDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<LoanApplication> LoanApplications => Set<LoanApplication>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<BlacklistedMobile> BlacklistedMobiles => Set<BlacklistedMobile>();
        public DbSet<BlacklistedDomain> BlacklistedDomains => Set<BlacklistedDomain>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(QuickLoanDbContext).Assembly);
        }
    }
}
