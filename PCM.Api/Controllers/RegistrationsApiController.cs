using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCM.Core.Entities;
using PCM.Infrastructure.Persistence;

namespace PCM.Api.Controllers
{
    [Authorize]
    [Route("api/registrations")] // Corrected route
    [ApiController]
    public class RegistrationsApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RegistrationsApiController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/registrations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Registration>>> GetRegistrations()
        {
            return await _context.Registrations
                .AsNoTracking()
                .Include(r => r.Member)
                .Include(r => r.Activity)
                .ToListAsync();
        }

        // POST: api/registrations
        [HttpPost]
        public async Task<ActionResult<Registration>> PostRegistration(Registration registration)
        {
            // ❌ Stop duplicate registrations
            var exists = await _context.Registrations.AnyAsync(r =>
                r.MemberId == registration.MemberId &&
                r.ActivityId == registration.ActivityId);

            if (exists)
            {
                return BadRequest("Member đã đăng ký Activity này rồi.");
            }

            registration.RegisteredAt = DateTime.UtcNow;

            _context.Registrations.Add(registration);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRegistrations), new { id = registration.Id }, registration);
        }
    }
}
