using System.Security.Cryptography;
using System.Text;
using QuickLoan.Application.Interfaces;

namespace QuickLoan.Infrastructure.Services;

public class ApplicationUrlGenerator : IApplicationUrlGenerator
{
    public string Generate(string firstName, string lastName, DateTime dateOfBirth)
    {
        var combined = Guid.NewGuid().ToString("N").Substring(0, 10).ToLower();        
        return combined.ToString().Replace("+", "-").Replace("/", "_").Replace("=", "");
    }
}
