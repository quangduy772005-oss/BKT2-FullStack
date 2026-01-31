using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCM.Core.Entities;
using PCM.Infrastructure.Persistence;

namespace PCM.Api.Controllers
{
    [Authorize]
    [Route("api/members")] // Corrected route
    [ApiController]
    public class MembersApiController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public MembersApiController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/members
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Member>>> GetMembers()
        {
            return await _context.Members
                .AsNoTracking()
                .ToListAsync();
        }

        // GET: api/members/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Member>> GetMember(int id)
        {
            var member = await _context.Members
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (member == null)
                return NotFound();

            return member;
        }

        // POST: api/members
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Member>> PostMember([FromBody] MemberCreateDto memberDto)
        {
            if (string.IsNullOrWhiteSpace(memberDto.Email))
                return BadRequest(new { message = "Email không được để trống" });

            // Using transaction to ensure both User and Member are created
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Create Identity User
                var user = new AppUser
                {
                    UserName = memberDto.Email,
                    Email = memberDto.Email,
                    FirstName = memberDto.FullName.Split(' ', 2)[0],
                    LastName = memberDto.FullName.Contains(' ') ? memberDto.FullName.Substring(memberDto.FullName.IndexOf(' ') + 1) : "",
                    IsActive = true
                };

                var userResult = await _userManager.CreateAsync(user, "Member@123"); // Default password
                if (!userResult.Succeeded)
                {
                    var errors = string.Join(". ", userResult.Errors.Select(e => e.Description));
                    return BadRequest(new { message = errors });
                }

                await _userManager.AddToRoleAsync(user, "Member");

                // 2. Create Member entity
                var member = new Member
                {
                    UserId = user.Id,
                    FullName = memberDto.FullName,
                    Email = memberDto.Email,
                    PhoneNumber = memberDto.PhoneNumber,
                    JoinDate = DateTime.UtcNow
                };

                _context.Members.Add(member);
                await _context.SaveChangesAsync();
                
                await transaction.CommitAsync();

                return Ok(member);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = "Đã xảy ra lỗi hệ thống: " + ex.Message });
            }
        }

        // DELETE: api/members/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMember(int id)
        {
            var member = await _context.Members.FindAsync(id);
            if (member == null)
                return NotFound();

            // Note: We might want to also delete the Identity user, 
            // but for safety we'll just remove the member record or deactivate.
            _context.Members.Remove(member);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    public class MemberCreateDto
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; }
    }
}
