namespace PCM.Application.DTOs;

#region Tournament DTOs

public class TournamentDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Type { get; set; } = null!;
    public string Format { get; set; } = null!;
    public string Status { get; set; } = null!;
    public decimal EntryFee { get; set; }
    public decimal PrizePool { get; set; }
    public string? Description { get; set; }
    public int ParticipantCount { get; set; }
}

public class CreateTournamentRequest
{
    public string Name { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Type { get; set; } = null!; // Duel, MiniGame, Professional
    public string Format { get; set; } = null!; // RoundRobin, Knockout, TeamBattle
    public decimal EntryFee { get; set; }
    public decimal PrizePool { get; set; }
    public string? Description { get; set; }
}

public class TournamentParticipantDto
{
    public int Id { get; set; }
    public int TournamentId { get; set; }
    public int MemberId { get; set; }
    public string MemberName { get; set; } = null!;
    public string? TeamName { get; set; }
    public string Status { get; set; } = null!;
    public int? SeedNo { get; set; }
}

public class JoinTournamentRequest
{
    public int TournamentId { get; set; }
    public string? TeamName { get; set; }
}

public class BracketNodeDto
{
    public int MatchId { get; set; }
    public int Round { get; set; }
    public string? Team1Name { get; set; }
    public string? Team2Name { get; set; }
    public int? Team1Score { get; set; }
    public int? Team2Score { get; set; }
    public string? Winner { get; set; }
    public List<int> NextMatchIds { get; set; } = new();
}

public class BracketDto
{
    public int TournamentId { get; set; }
    public List<BracketNodeDto> Nodes { get; set; } = new();
}

public class GenerateBracketRequest
{
    public int TournamentId { get; set; }
    public bool UseSeeding { get; set; } = false;
}

#endregion

#region Match DTOs

public class MatchDto
{
    public int Id { get; set; }
    public DateTime MatchDate { get; set; }
    public string MatchFormat { get; set; } = null!;
    public string Team1Player1 { get; set; } = null!;
    public string? Team1Player2 { get; set; }
    public string Team2Player1 { get; set; } = null!;
    public string? Team2Player2 { get; set; }
    public int ScoreTeam1 { get; set; }
    public int ScoreTeam2 { get; set; }
    public string Result { get; set; } = null!;
    public double? EloChange { get; set; }
    public int? TournamentId { get; set; }
}

public class CreateMatchRequest
{
    public DateTime MatchDate { get; set; }
    public string MatchFormat { get; set; } = null!; // Singles, Doubles
    public int Team1Player1Id { get; set; }
    public int? Team1Player2Id { get; set; }
    public int Team2Player1Id { get; set; }
    public int? Team2Player2Id { get; set; }
    public int? TournamentId { get; set; }
}

public class UpdateMatchResultRequest
{
    public int MatchId { get; set; }
    public int ScoreTeam1 { get; set; }
    public int ScoreTeam2 { get; set; }
}

#endregion
