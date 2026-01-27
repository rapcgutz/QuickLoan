namespace QuickLoan.Domain.Entities;

public class BlacklistedMobile
{
    public Guid Id { get; private set; }
    public string Mobile { get; private set; } = string.Empty;
    public DateTime BlockedAt { get; private set; }
    public Guid BlockedByUserId { get; private set; }
    public string? Reason { get; private set; }
    
    public virtual User BlockedByUser { get; private set; } = null!;
    
    private BlacklistedMobile() { }
    
    public static BlacklistedMobile Create(string mobile, Guid blockedByUserId, string? reason = null)
    {
        if (string.IsNullOrWhiteSpace(mobile))
            throw new ArgumentException("Mobile cannot be empty", nameof(mobile));
        
        return new BlacklistedMobile
        {
            Id = Guid.NewGuid(),
            Mobile = mobile.Trim(),
            BlockedAt = DateTime.UtcNow,
            BlockedByUserId = blockedByUserId,
            Reason = reason
        };
    }
}
