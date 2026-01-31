using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCM.Core.Entities;
using PCM.Infrastructure.Persistence;

namespace PCM.Api.Controllers
{
    [Authorize]
    [Route("api/clubs")] // Corrected route
    [ApiController]
    public class ClubsApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClubsApiController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/clubs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Club>>> GetClubs()
        {
            return await _context.Clubs
                .AsNoTracking()
                .ToListAsync();
        }

        // POST: api/clubs
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Club>> PostClub(Club club)
        {
            _context.Clubs.Add(club);
            await _context.SaveChangesAsync();
            return Ok(club);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClub(int id, Club club)
        {
            if (id != club.Id) return BadRequest();
            _context.Entry(club).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClub(int id)
        {
            var club = await _context.Clubs.FindAsync(id);
            if (club == null) return NotFound();
            _context.Clubs.Remove(club);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
