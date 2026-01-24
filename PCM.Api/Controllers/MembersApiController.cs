using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCM.Api.Data;
using PCM.Api.Models;

namespace PCM.Api.Controllers
{
    [Authorize] // 🔐 BẮT BUỘC ĐĂNG NHẬP
    [Route("api/[controller]")]
    [ApiController]
    public class MembersApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MembersApiController(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // GET: api/MembersApi
        // User + Admin đều xem được
        // =========================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Member>>> GetMembers()
        {
            return await _context.Members
                .Include(m => m.Club)
                .ToListAsync();
        }

        // =========================
        // GET: api/MembersApi/5
        // User + Admin
        // =========================
        [HttpGet("{id}")]
        public async Task<ActionResult<Member>> GetMember(int id)
        {
            var member = await _context.Members
                .Include(m => m.Club)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (member == null)
                return NotFound();

            return member;
        }

        // =========================
        // POST: api/MembersApi
        // CHỈ ADMIN
        // =========================
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Member>> PostMember(Member member)
        {
            _context.Members.Add(member);
            await _context.SaveChangesAsync();

            return Ok(member);
            // Nếu muốn chuẩn REST:
            // return CreatedAtAction(nameof(GetMember), new { id = member.Id }, member);
        }

        // =========================
        // PUT: api/MembersApi/5
        // CHỈ ADMIN
        // =========================
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMember(int id, Member member)
        {
            if (id != member.Id)
                return BadRequest();

            _context.Entry(member).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Members.Any(e => e.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // =========================
        // DELETE: api/MembersApi/5
        // CHỈ ADMIN
        // =========================
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMember(int id)
        {
            var member = await _context.Members.FindAsync(id);
            if (member == null)
                return NotFound();

            _context.Members.Remove(member);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
