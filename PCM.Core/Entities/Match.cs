using PCM.Core.Enums;

namespace PCM.Core.Entities;

public class Match : BaseEntity
{
    public DateTime MatchDate { get; set; }
    public bool IsRanked { get; set; } = true;
    public MatchFormat MatchFormat { get; set; }

    // Team 1
    public int? Team1Player1Id { get; set; }
    public int? Team1Player2Id { get; set; }

    // Team 2
    public int? Team2Player1Id { get; set; }
    public int? Team2Player2Id { get; set; }

    // Score
    public int ScoreTeam1 { get; set; }
    public int ScoreTeam2 { get; set; }

    // Result
    public MatchResult Result { get; set; } = MatchResult.None;
    public double? EloChange { get; set; } // ELO points change for Team1

    // Tournament reference
    public int? TournamentId { get; set; }

    // Navigation
    public virtual Member? Team1Player1 { get; set; }
    public virtual Member? Team1Player2 { get; set; }
    public virtual Member? Team2Player1 { get; set; }
    public virtual Member? Team2Player2 { get; set; }
    public virtual Tournament? Tournament { get; set; }
}

/// <summary>
/// Mini game result for team battles
/// </summary>
public class MiniGameResult : BaseEntity
{
    public int MatchId { get; set; }
    public int TeamAPoints { get; set; }
    public int TeamBPoints { get; set; }
    public MatchResult TeamWinner { get; set; }

    // Navigation
    public virtual Match Match { get; set; } = null!;
}
