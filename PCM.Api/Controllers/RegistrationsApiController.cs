using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCM.Api.Data;
using PCM.Api.Models;

namespace PCM.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationsApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RegistrationsApiController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/RegistrationsApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Registration>>> GetRegistrations()
        {
            return await _context.Registrations
                .Include(r => r.Member)
                .Include(r => r.Activity)
                .ToListAsync();
        }

        // POST: api/RegistrationsApi
        [HttpPost]
        public async Task<ActionResult<Registration>> PostRegistration(Registration registration)
        {
            // ❌ Chặn đăng ký trùng
            var exists = await _context.Registrations.AnyAsync(r =>
                r.MemberId == registration.MemberId &&
                r.ActivityId == registration.ActivityId);

            if (exists)
            {
                return BadRequest("Member đã đăng ký Activity này rồi.");
            }

            registration.RegisteredAt = DateTime.Now;

            _context.Registrations.Add(registration);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRegistrations), new { id = registration.Id }, registration);
        }
    }
}
