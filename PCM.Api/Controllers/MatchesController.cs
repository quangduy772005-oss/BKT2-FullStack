using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PCM.Application.Services;
using PCM.Core.Enums;

namespace PCM.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MatchesController : ControllerBase
    {
        private readonly MatchService _matchService;

        public MatchesController(MatchService matchService)
        {
            _matchService = matchService;
        }

        /// <summary>
        /// Record match result
        /// </summary>
        [HttpPost("record-result/{matchId}")]
        [Authorize(Roles = "Admin,Referee")]
        public async Task<IActionResult> RecordResult(int matchId, [FromBody] RecordMatchResultDto request)
        {
            try
            {
                var match = await _matchService.RecordMatchResultAsync(
                    matchId,
                    (MatchResultEnum)request.Result,
                    request.WinnerScore,
                    request.LoserScore
                );

                if (match == null)
                    return BadRequest("Không thể ghi nhận kết quả");

                return Ok(new
                {
                    matchId = match.Id,
                    eloChange = match.EloChange,
                    message = "Kết quả trận đấu đã được ghi nhận"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get match statistics for current user
        /// </summary>
        [HttpGet("my-statistics")]
        public async Task<IActionResult> GetMyStatistics()
        {
            try
            {
                var memberId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                if (memberId == 0)
                    return Unauthorized();

                var stats = await _matchService.GetMemberMatchStatisticsAsync(memberId);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get leaderboard for a tournament
        /// </summary>
        [HttpGet("leaderboard/{tournamentId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetLeaderboard(int tournamentId, [FromQuery] int limit = 100)
        {
            try
            {
                var leaderboard = await _matchService.GetLeaderboardAsync(tournamentId, limit);
                return Ok(leaderboard.Select(x => new
                {
                    rank = x.rank,
                    memberId = x.member.Id,
                    memberName = x.member.User?.UserName,
                    elo = x.member.RankELO
                }));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get tournament matches
        /// </summary>
        [HttpGet("tournament/{tournamentId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTournamentMatches(int tournamentId)
        {
            try
            {
                var matches = await _matchService.GetTournamentMatchesAsync(tournamentId);
                return Ok(matches);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    public class RecordMatchResultDto
    {
        public int Result { get; set; }
        public int? WinnerScore { get; set; }
        public int? LoserScore { get; set; }
    }
}
