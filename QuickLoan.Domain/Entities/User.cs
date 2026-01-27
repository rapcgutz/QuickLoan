using QuickLoan.Domain.Enums;

namespace QuickLoan.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public DateTime DateOfBirth { get; private set; }
    public string Mobile { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    
    public virtual ICollection<LoanApplication> LoanApplications { get; private set; } = new List<LoanApplication>();
    public virtual ICollection<RefreshToken> RefreshTokens { get; private set; } = new List<RefreshToken>();
    
    private User() { }
    
    public static User Create(
        string email,
        string passwordHash,
        string firstName,
        string lastName,
        DateTime dateOfBirth,
        string mobile,
        UserRole role = UserRole.StandardUser)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));
            
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty", nameof(passwordHash));
            
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));
            
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));
            
        if (!IsAtLeast18YearsOld(dateOfBirth))
            throw new ArgumentException("User must be at least 18 years old", nameof(dateOfBirth));
            
        if (string.IsNullOrWhiteSpace(mobile))
            throw new ArgumentException("Mobile cannot be empty", nameof(mobile));
        
        return new User
        {
            Id = Guid.NewGuid(),
            Email = email.ToLower().Trim(),
            PasswordHash = passwordHash,
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            DateOfBirth = dateOfBirth.Date,
            Mobile = mobile.Trim(),
            Role = role,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
    
    public static bool IsAtLeast18YearsOld(DateTime dateOfBirth)
    {
        var today = DateTime.Today;
        var age = today.Year - dateOfBirth.Year;
        
        if (dateOfBirth.Date > today.AddYears(-age))
            age--;
        
        return age >= 18;
    }
    
    public void UpdateProfile(string firstName, string lastName, string mobile)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));
            
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));
            
        if (string.IsNullOrWhiteSpace(mobile))
            throw new ArgumentException("Mobile cannot be empty", nameof(mobile));
        
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Mobile = mobile.Trim();
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void ChangePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("Password hash cannot be empty", nameof(newPasswordHash));
        
        PasswordHash = newPasswordHash;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public bool IsAdmin() => Role == UserRole.Admin;
    
    public string GetFullName() => $"{FirstName} {LastName}";
}
