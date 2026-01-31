using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PCM.Application.Services;
using PCM.Core.Enums;

namespace PCM.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TournamentsController : ControllerBase
    {
        private readonly TournamentService _tournamentService;

        public TournamentsController(TournamentService tournamentService)
        {
            _tournamentService = tournamentService;
        }

        /// <summary>
        /// Create a new tournament
        /// </summary>
        [HttpPost("create")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTournament([FromBody] CreateTournamentDto request)
        {
            try
            {
                var tournament = await _tournamentService.CreateTournamentAsync(
                    request.Name ?? "",
                    request.Description ?? "",
                    (PCM.Core.Enums.TournamentType)request.Type,
                    (PCM.Core.Enums.TournamentFormat)request.Format,
                    request.StartDate,
                    request.EndDate,
                    request.MaxParticipants,
                    request.EntryFee,
                    request.PrizePool,
                    request.Location ?? ""
                );

                if (tournament == null)
                    return BadRequest("Không thể tạo giải đấu");

                return Ok(new { id = tournament.Id, message = "Tạo giải đấu thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Join a tournament
        /// </summary>
        [HttpPost("join/{tournamentId}")]
        public async Task<IActionResult> JoinTournament(int tournamentId)
        {
            try
            {
                var memberId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                if (memberId == 0)
                    return Unauthorized();

                var participant = await _tournamentService.JoinTournamentAsync(tournamentId, memberId);
                if (participant == null)
                    return BadRequest("Không thể đăng ký giải đấu");
                    
                return Ok(new { id = participant.Id, message = "Đăng ký giải đấu thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Generate tournament bracket
        /// </summary>
        [HttpPost("generate-bracket/{tournamentId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GenerateBracket(int tournamentId)
        {
            try
            {
                var bracket = await _tournamentService.GenerateKnockoutBracketAsync(tournamentId);
                return Ok(new { message = "Bracket đã được tạo thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Start tournament (close registration)
        /// </summary>
        [HttpPost("start/{tournamentId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> StartTournament(int tournamentId)
        {
            try
            {
                await _tournamentService.StartTournamentAsync(tournamentId);
                return Ok("Bắt đầu giải đấu thành công");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// End tournament
        /// </summary>
        [HttpPost("end/{tournamentId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EndTournament(int tournamentId)
        {
            try
            {
                await _tournamentService.EndTournamentAsync(tournamentId);
                return Ok("Kết thúc giải đấu thành công");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get all active tournaments
        /// </summary>
        [HttpGet("active")]
        [AllowAnonymous]
        public async Task<IActionResult> GetActiveTournaments()
        {
            try
            {
                var tournaments = await _tournamentService.GetActiveTournamentsAsync();
                return Ok(tournaments);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get tournament details
        /// </summary>
        [HttpGet("{tournamentId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTournament(int tournamentId)
        {
            try
            {
                var tournament = await _tournamentService.GetTournamentAsync(tournamentId);
                if (tournament == null)
                    return NotFound();

                return Ok(tournament);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get tournament participants
        /// </summary>
        [HttpGet("{tournamentId}/participants")]
        [AllowAnonymous]
        public async Task<IActionResult> GetParticipants(int tournamentId)
        {
            try
            {
                var participants = await _tournamentService.GetParticipantsAsync(tournamentId);
                return Ok(participants);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    public class CreateTournamentDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Type { get; set; }
        public int Format { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int MaxParticipants { get; set; }
        public decimal EntryFee { get; set; }
        public decimal PrizePool { get; set; }
        public string? Location { get; set; }
    }
}
