using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCM.Api.Data;
using PCM.Api.Models;

namespace PCM.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClubsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClubsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Clubs
        [HttpGet]
        public async Task<IActionResult> GetClubs()
        {
            var clubs = await _context.Clubs.ToListAsync();
            return Ok(clubs);
        }

        // GET: api/Clubs/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetClub(int id)
        {
            var club = await _context.Clubs.FindAsync(id);
            if (club == null)
                return NotFound();

            return Ok(club);
        }

        // POST: api/Clubs
        [HttpPost]
        public async Task<IActionResult> CreateClub(Club club)
        {
            _context.Clubs.Add(club);
            await _context.SaveChangesAsync();
            return Ok(club);
        }

        // PUT: api/Clubs/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClub(int id, Club club)
        {
            if (id != club.Id)
                return BadRequest();

            _context.Entry(club).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Clubs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClub(int id)
        {
            var club = await _context.Clubs.FindAsync(id);
            if (club == null)
                return NotFound();

            _context.Clubs.Remove(club);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
