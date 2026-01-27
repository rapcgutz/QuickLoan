using System.Security.Cryptography;
using System.Text;
using QuickLoan.Application.Interfaces;

namespace QuickLoan.Infrastructure.Services;

public class ApplicationUrlGenerator : IApplicationUrlGenerator
{
    public string Generate(string firstName, string lastName, DateTime dateOfBirth)
    {
        var combined = $"{firstName.ToLower().Trim()}-{lastName.ToLower().Trim()}-{dateOfBirth:yyyyMMdd}";
        
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combined));
        var hash = Convert.ToBase64String(hashBytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "")
            .Substring(0, 12);
        
        return hash.ToLower();
    }
}
