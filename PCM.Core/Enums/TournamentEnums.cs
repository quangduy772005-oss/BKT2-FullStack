namespace PCM.Core.Enums;

public enum TournamentType
{
    Duel = 0,
    MiniGame = 1,
    Professional = 2
}

public enum TournamentFormat
{
    RoundRobin = 0,
    Knockout = 1,
    TeamBattle = 2,
    DoubleElimination = 3
}

public enum TournamentStatus
{
    Open = 0,
    Ongoing = 1,
    Finished = 2,
    Cancelled = 3
}

public enum ParticipantStatus
{
    Registered = 0,
    Paid = 1,
    Eliminated = 2,
    Withdrawn = 3
}

public enum MatchFormat
{
    Singles = 0,
    Doubles = 1
}

public enum MatchResult
{
    None = 0,
    Team1Win = 1,
    Team2Win = 2,
    Draw = 3
}

public enum BracketType
{
    WinnerBracket = 0,
    LoserBracket = 1
}
