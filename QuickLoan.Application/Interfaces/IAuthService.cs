using QuickLoan.Domain.Entities;

namespace QuickLoan.Application.Interfaces;

public interface IAuthService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    bool VerifyPassword(string password, string passwordHash);
    string HashPassword(string password);
}
