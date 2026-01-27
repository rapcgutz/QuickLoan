using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickLoan.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuickLoan.Infrastructure.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.HasIndex(u => u.Email).IsUnique();

            builder.Property(u => u.PasswordHash).IsRequired();
            builder.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
            builder.Property(u => u.LastName).IsRequired().HasMaxLength(100);
            builder.Property(u => u.Mobile).IsRequired().HasMaxLength(20);
            builder.Property(u => u.DateOfBirth).IsRequired();
            builder.Property(u => u.Role).IsRequired();

            builder.Property(u => u.CreatedAt).IsRequired();
            builder.Property(u => u.UpdatedAt).IsRequired();

            builder.HasMany(u => u.LoanApplications)
                .WithOne(la => la.User)
                .HasForeignKey(la => la.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(u => u.RefreshTokens)
                .WithOne(rt => rt.User)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
