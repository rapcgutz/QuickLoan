using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuickLoan.Application.Interfaces;
using QuickLoan.Infrastructure.Data;
using QuickLoan.Infrastructure.Repositories;
using QuickLoan.Infrastructure.Services;

namespace QuickLoan.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<QuickLoanDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(QuickLoanDbContext).Assembly.FullName)));

        // Repositories
        services.AddScoped<ILoanApplicationRepository, LoanApplicationRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IBlacklistRepository, BlacklistRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        // Services
        services.AddScoped<IQuotationService, QuotationService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IApplicationUrlGenerator, ApplicationUrlGenerator>();

        return services;
    }
}