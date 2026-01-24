using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCM.Api.Data;
using PCM.Api.Models;

namespace PCM.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClubsApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClubsApiController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Club>>> GetClubs()
        {
            return await _context.Clubs.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Club>> GetClub(int id)
        {
            var club = await _context.Clubs.FindAsync(id);
            if (club == null) return NotFound();
            return club;
        }

        [HttpPost]
        public async Task<ActionResult<Club>> CreateClub(Club club)
        {
            _context.Clubs.Add(club);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetClub), new { id = club.Id }, club);
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
