using PCM.Core.Enums;

namespace PCM.Core.Entities;

public class Tournament : BaseEntity
{
    public string Name { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public TournamentType Type { get; set; }
    public TournamentFormat Format { get; set; }
    public TournamentStatus Status { get; set; } = TournamentStatus.Open;
    public decimal EntryFee { get; set; } = 0;
    public decimal PrizePool { get; set; } = 0;
    public string? ConfigData { get; set; } // JSON configuration
    public string? Description { get; set; }

    // Navigation
    public virtual ICollection<TournamentParticipant> Participants { get; set; } = new List<TournamentParticipant>();
    public virtual ICollection<TournamentMatch> Matches { get; set; } = new List<TournamentMatch>();
}

public class TournamentParticipant : BaseEntity
{
    public int TournamentId { get; set; }
    public int MemberId { get; set; }
    public string? TeamName { get; set; }
    public ParticipantStatus Status { get; set; } = ParticipantStatus.Registered;
    public int? SeedNo { get; set; } // Seed number for bracket generation
    public int? InitialRanking { get; set; }

    // Navigation
    public virtual Tournament Tournament { get; set; } = null!;
    public virtual Member Member { get; set; } = null!;
}

/// <summary>
/// Represents a match in the tournament bracket (Knockout system)
/// </summary>
public class TournamentMatch : BaseEntity
{
    public int TournamentId { get; set; }
    public int? MatchId { get; set; } // FK to actual Match
    public int? NextMatchId { get; set; } // Next match in bracket
    public int? ParentMatchId { get; set; } // Parent match in bracket
    public int Round { get; set; } // 1=Group, 2=QF, 3=SF, 4=Final
    public BracketType BracketType { get; set; } = BracketType.WinnerBracket;

    // Navigation
    public virtual Tournament Tournament { get; set; } = null!;
    public virtual Match? Match { get; set; }
    public virtual TournamentMatch? NextMatch { get; set; }
    public virtual TournamentMatch? ParentMatch { get; set; }
}
