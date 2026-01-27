namespace QuickLoan.Domain.Entities;

public class BlacklistedDomain
{
    public Guid Id { get; private set; }
    public string Domain { get; private set; } = string.Empty;
    public DateTime BlockedAt { get; private set; }
    public Guid BlockedByUserId { get; private set; }
    public string? Reason { get; private set; }
    
    public virtual User BlockedByUser { get; private set; } = null!;
    
    private BlacklistedDomain() { }
    
    public static BlacklistedDomain Create(string domain, Guid blockedByUserId, string? reason = null)
    {
        if (string.IsNullOrWhiteSpace(domain))
            throw new ArgumentException("Domain cannot be empty", nameof(domain));
        
        return new BlacklistedDomain
        {
            Id = Guid.NewGuid(),
            Domain = domain.ToLower().Trim(),
            BlockedAt = DateTime.UtcNow,
            BlockedByUserId = blockedByUserId,
            Reason = reason
        };
    }
}
