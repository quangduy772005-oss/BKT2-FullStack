using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCM.Api.Data;
using PCM.Api.Models;

namespace PCM.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivitiesApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ActivitiesApiController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/ActivitiesApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Activity>>> GetActivities()
        {
            return await _context.Activities
                .Include(a => a.Club)
                .ToListAsync();
        }

        // GET: api/ActivitiesApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Activity>> GetActivity(int id)
        {
            var activity = await _context.Activities
                .Include(a => a.Club)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (activity == null)
            {
                return NotFound();
            }

            return activity;
        }

        // POST: api/ActivitiesApi
        [HttpPost]
        public async Task<ActionResult<Activity>> PostActivity(Activity activity)
        {
            // kiểm tra Club có tồn tại không
            var clubExists = await _context.Clubs.AnyAsync(c => c.Id == activity.ClubId);
            if (!clubExists)
            {
                return BadRequest("ClubId không tồn tại");
            }

            _context.Activities.Add(activity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetActivity), new { id = activity.Id }, activity);
        }
    }
}
