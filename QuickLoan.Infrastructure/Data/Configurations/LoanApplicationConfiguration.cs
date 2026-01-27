using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickLoan.Domain.Entities;

namespace QuickLoan.Infrastructure.Data.Configurations;

public class LoanApplicationConfiguration : IEntityTypeConfiguration<LoanApplication>
{
    public void Configure(EntityTypeBuilder<LoanApplication> builder)
    {
        builder.ToTable("LoanApplications");

        builder.HasKey(la => la.Id);

        builder.Property(la => la.ApplicationUrl)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(la => la.ApplicationUrl).IsUnique();

        builder.Property(la => la.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(la => la.LastName).IsRequired().HasMaxLength(100);
        builder.Property(la => la.Email).IsRequired().HasMaxLength(255);
        builder.Property(la => la.Mobile).IsRequired().HasMaxLength(20);

        builder.Property(la => la.AmountRequired).HasColumnType("decimal(18,2)");
        builder.Property(la => la.MonthlyRepayment).HasColumnType("decimal(18,2)");
        builder.Property(la => la.EstablishmentFee).HasColumnType("decimal(18,2)");
        builder.Property(la => la.TotalInterest).HasColumnType("decimal(18,2)");
        builder.Property(la => la.TotalRepayment).HasColumnType("decimal(18,2)");

        builder.Property(la => la.AdminNotes).HasMaxLength(1000);

        builder.Property(la => la.Title).IsRequired();
        builder.Property(la => la.ProductType).IsRequired();
        builder.Property(la => la.Status).IsRequired();
    }
}