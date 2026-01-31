namespace PCM.Core.Entities;

public class Member : BaseEntity
{
    public string UserId { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public DateTime JoinDate { get; set; }

    // Ranking & Wallet
    public double RankELO { get; set; } = 1200; // Default ELO rating
    public decimal WalletBalance { get; set; } = 0;

    // Profile
    public string? AvatarUrl { get; set; }
    public byte[]? RowVersion { get; set; } // For optimistic locking

    // Navigation
    public int? ClubId { get; set; }
    public virtual Club? Club { get; set; }
    public virtual AppUser User { get; set; } = null!;
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public virtual ICollection<WalletTransaction> Transactions { get; set; } = new List<WalletTransaction>();
    public virtual ICollection<Match> MatchesAsTeam1Player1 { get; set; } = new List<Match>();
    public virtual ICollection<Match> MatchesAsTeam1Player2 { get; set; } = new List<Match>();
    public virtual ICollection<Match> MatchesAsTeam2Player1 { get; set; } = new List<Match>();
    public virtual ICollection<Match> MatchesAsTeam2Player2 { get; set; } = new List<Match>();
    public virtual ICollection<TournamentParticipant> TournamentParticipations { get; set; } = new List<TournamentParticipant>();
    public virtual ICollection<DepositRequest> DepositRequests { get; set; } = new List<DepositRequest>();
    public virtual ICollection<Registration> Registrations { get; set; } = new List<Registration>();
}
