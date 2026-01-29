using System.ComponentModel.DataAnnotations;

namespace QuickLoan.Web.Models;

// Authentication Models
public class LoginRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}

public class RegisterRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please confirm your password")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "First name is required")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Date of birth is required")]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }

    [Required(ErrorMessage = "Mobile is required")]
    [Phone(ErrorMessage = "Invalid phone number")]
    public string Mobile { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string Role { get; set; } = string.Empty;
    public UserDto User { get; set; } = null!;
}

public class RegisterResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string Role { get; set; } = string.Empty;
    public UserDto User { get; set; } = null!;
}

public class TokenResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Mobile { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

// Loan Models
public class QuotationRequest
{
    [Required(ErrorMessage = "Amount is required")]
    [Range(1000, 1000000, ErrorMessage = "Amount must be between $1,000 and $1,000,000")]
    public decimal AmountRequired { get; set; }

    [Required(ErrorMessage = "Term is required")]
    [Range(1, 60, ErrorMessage = "Term must be between 1 and 60 months")]
    public int Term { get; set; }

    [Required(ErrorMessage = "Product type is required")]
    public string ProductType { get; set; } = "ProductC";

    [Required(ErrorMessage = "Title is required")]
    public string Title { get; set; } = "Mr";

    [Required(ErrorMessage = "First name is required")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Date of birth is required")]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }

    [Required(ErrorMessage = "Mobile is required")]
    [Phone(ErrorMessage = "Invalid phone number")]
    public string Mobile { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = string.Empty;
}

public class QuotationResponse
{
    public string ApplicationUrl { get; set; } = string.Empty;
    public string RedirectUrl { get; set; } = string.Empty;
    public decimal AmountRequired { get; set; }
    public int Term { get; set; }
    public string ProductType { get; set; } = string.Empty;
    public decimal MonthlyRepayment { get; set; }
    public decimal EstablishmentFee { get; set; }
    public decimal TotalInterest { get; set; }
    public decimal TotalRepayment { get; set; }
    public decimal AnnualInterestRate { get; set; }
    public int InterestFreeMonths { get; set; }
}

public class LoanApplicationRequest
{
    [Required]
    [Range(1000, 1000000)]
    public decimal AmountRequired { get; set; }

    [Required]
    [Range(1, 60)]
    public int Term { get; set; }

    [Required]
    public string ProductType { get; set; } = string.Empty;

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }

    [Required]
    [Phone]
    public string Mobile { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}

public class UpdateLoanRequest
{
    [Required]
    [Range(1000, 1000000)]
    public decimal AmountRequired { get; set; }

    [Required]
    [Range(1, 60)]
    public int Term { get; set; }

    [Required]
    public string ProductType { get; set; } = string.Empty;
}

public class LoanApplicationResponse
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public string ApplicationUrl { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Mobile { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal AmountRequired { get; set; }
    public int Term { get; set; }
    public string ProductType { get; set; } = string.Empty;
    public decimal MonthlyRepayment { get; set; }
    public decimal EstablishmentFee { get; set; }
    public decimal TotalInterest { get; set; }
    public decimal TotalRepayment { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public string? AdminNotes { get; set; }
}

// Admin Models
public class BlockedMobileResponse
{
    public string Mobile { get; set; } = string.Empty;
    public DateTime BlockedAt { get; set; }
    public string BlockedBy { get; set; } = string.Empty;
}

public class BlockedDomainResponse
{
    public string Domain { get; set; } = string.Empty;
    public DateTime BlockedAt { get; set; }
    public string BlockedBy { get; set; } = string.Empty;
}

public class BlockMobileRequest
{
    [Required]
    [Phone]
    public string Mobile { get; set; } = string.Empty;

    public string? Reason { get; set; }
}

public class BlockDomainRequest
{
    [Required]
    [ValidDomain(ErrorMessage = "Please enter a valid domain (e.g. gmail.com).")]
    public string Domain { get; set; } = string.Empty;

    public string? Reason { get; set; }
}

public class BlacklistResponse
{
    public List<string> BlockedMobiles { get; set; }
    public List<string> BlockedDomains { get; set; }
}