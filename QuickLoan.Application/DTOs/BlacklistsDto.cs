namespace QuickLoan.Application.DTOs;

public class BlacklistsDto
{
    public List<string> BlockedMobiles { get; set; } = new();
    public List<string> BlockedDomains { get; set; } = new();
}
