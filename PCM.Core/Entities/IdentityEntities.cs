using Microsoft.AspNetCore.Identity;

namespace PCM.Core.Entities;

public class AppUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public DateTime JoinDate { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    // Navigation
    public virtual Member? Member { get; set; }
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}

public class RefreshToken : BaseEntity
{
    public string UserId { get; set; } = null!;
    public string Token { get; set; } = null!;
    public string JwtId { get; set; } = null!;
    public bool IsUsed { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime AddedDate { get; set; }
    public DateTime ExpiryDate { get; set; }

    // Navigation
    public virtual AppUser AppUser { get; set; } = null!;
}
