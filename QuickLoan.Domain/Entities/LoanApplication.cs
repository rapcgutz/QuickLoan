using QuickLoan.Domain.Enums;

namespace QuickLoan.Domain.Entities;

public class LoanApplication
{
    public Guid Id { get; private set; }
    public Guid? UserId { get; private set; }
    public string ApplicationUrl { get; private set; } = string.Empty;
    
    // Applicant Information
    public Title Title { get; private set; }
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public DateTime DateOfBirth { get; private set; }
    public string Mobile { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    
    // Loan Details
    public decimal AmountRequired { get; private set; }
    public int Term { get; private set; }
    public ProductType ProductType { get; private set; }
    
    // Calculated Fields
    public decimal MonthlyRepayment { get; private set; }
    public decimal EstablishmentFee { get; private set; }
    public decimal TotalInterest { get; private set; }
    public decimal TotalRepayment { get; private set; }
    
    // Status
    public ApplicationStatus Status { get; private set; }
    
    // Audit
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime? SubmittedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public string? AdminNotes { get; private set; }
    
    public virtual User? User { get; private set; }
    
    private LoanApplication() { }
    
    public static LoanApplication Create(
        Title title,
        string firstName,
        string lastName,
        DateTime dateOfBirth,
        string mobile,
        string email,
        decimal amountRequired,
        int term,
        ProductType productType,
        string applicationUrl,
        Guid? userId = null)
    {
        ValidateInputs(firstName, lastName, mobile, email, amountRequired, term);
        
        if (!User.IsAtLeast18YearsOld(dateOfBirth))
            throw new ArgumentException("Applicant must be at least 18 years old", nameof(dateOfBirth));
        
        return new LoanApplication
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ApplicationUrl = applicationUrl,
            Title = title,
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            DateOfBirth = dateOfBirth.Date,
            Mobile = mobile.Trim(),
            Email = email.ToLower().Trim(),
            AmountRequired = amountRequired,
            Term = term,
            ProductType = productType,
            Status = ApplicationStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
    
    public void SetQuotation(decimal monthlyRepayment, decimal establishmentFee, decimal totalInterest, decimal totalRepayment)
    {
        if (monthlyRepayment < 0) throw new ArgumentException("Monthly repayment cannot be negative");
        if (establishmentFee < 0) throw new ArgumentException("Establishment fee cannot be negative");
        if (totalInterest < 0) throw new ArgumentException("Total interest cannot be negative");
        if (totalRepayment < 0) throw new ArgumentException("Total repayment cannot be negative");
        
        MonthlyRepayment = Math.Round(monthlyRepayment, 2);
        EstablishmentFee = Math.Round(establishmentFee, 2);
        TotalInterest = Math.Round(totalInterest, 2);
        TotalRepayment = Math.Round(totalRepayment, 2);
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void Update(decimal amountRequired, int term, ProductType productType, Title? title = null, string? mobile = null, string? email = null)
    {
        if (Status != ApplicationStatus.Draft)
            throw new InvalidOperationException("Cannot update application that has been submitted");
        
        if (amountRequired <= 0) throw new ArgumentException("Amount must be greater than zero");
        if (term <= 0) throw new ArgumentException("Term must be greater than zero");
        
        AmountRequired = amountRequired;
        Term = term;
        ProductType = productType;
        
        if (title.HasValue) Title = title.Value;
        if (!string.IsNullOrWhiteSpace(mobile)) Mobile = mobile.Trim();
        if (!string.IsNullOrWhiteSpace(email)) Email = email.ToLower().Trim();
        
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void Submit(Guid userId)
    {
        if (Status != ApplicationStatus.Draft)
            throw new InvalidOperationException("Only draft applications can be submitted");
        
        UserId = userId;
        Status = ApplicationStatus.Submitted;
        SubmittedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void Approve(string? notes = null)
    {
        if (Status != ApplicationStatus.Submitted && Status != ApplicationStatus.UnderReview)
            throw new InvalidOperationException("Can only approve submitted or under-review applications");
        
        Status = ApplicationStatus.Approved;
        ProcessedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        AdminNotes = notes;
    }
    
    public void Reject(string reason)
    {
        if (Status != ApplicationStatus.Submitted && Status != ApplicationStatus.UnderReview)
            throw new InvalidOperationException("Can only reject submitted or under-review applications");
        
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Rejection reason is required");
        
        Status = ApplicationStatus.Rejected;
        ProcessedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        AdminNotes = reason;
    }
    
    public void SetUnderReview()
    {
        if (Status != ApplicationStatus.Submitted)
            throw new InvalidOperationException("Can only review submitted applications");
        
        Status = ApplicationStatus.UnderReview;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public bool CanBeEdited() => Status == ApplicationStatus.Draft;
    public bool CanBeSubmitted() => Status == ApplicationStatus.Draft && UserId.HasValue;
    public string GetFullName() => $"{Title} {FirstName} {LastName}";
    
    private static void ValidateInputs(string firstName, string lastName, string mobile, string email, decimal amountRequired, int term)
    {
        if (string.IsNullOrWhiteSpace(firstName)) throw new ArgumentException("First name cannot be empty");
        if (string.IsNullOrWhiteSpace(lastName)) throw new ArgumentException("Last name cannot be empty");
        if (string.IsNullOrWhiteSpace(mobile)) throw new ArgumentException("Mobile cannot be empty");
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email cannot be empty");
        if (amountRequired <= 0) throw new ArgumentException("Amount must be greater than zero");
        if (term <= 0) throw new ArgumentException("Term must be greater than zero");
    }
}
