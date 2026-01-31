using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PCM.Core.Entities;
using PCM.Application.DTOs;
using PCM.Infrastructure.Persistence;

namespace PCM.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public AuthController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IConfiguration configuration,
            AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _context = context;
        }

        /// <summary>
        /// Login user
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return Unauthorized("Tên đăng nhập hoặc mật khẩu không chính xác");

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
                return Unauthorized("Tên đăng nhập hoặc mật khẩu không chính xác");

            var member = await _context.Members
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Email == request.Email);

            var token = await GenerateJwtToken(user, member?.Id ?? 0);
            var refreshToken = GenerateRefreshToken();

            user.RefreshTokens.Add(new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                JwtId = string.Empty,
                IsUsed = false,
                IsRevoked = false,
                AddedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(7)
            });

            await _userManager.UpdateAsync(user);

            return Ok(new LoginResponse
            {
                AccessToken = token,
                RefreshToken = refreshToken,
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email ?? "",
                    FullName = string.Join(' ', new[] { user.FirstName, user.LastName }.Where(s => !string.IsNullOrEmpty(s)))
                }
            });
        }

        /// <summary>
        /// Register new user
        /// </summary>
        [HttpPost("register")]
        public async Task<ActionResult<LoginResponse>> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = await _userManager.FindByEmailAsync(request.Email ?? "");
            if (existingUser != null)
                return BadRequest("Tên đăng nhập đã tồn tại");

            var names = (request.FullName ?? string.Empty).Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var first = names.Length > 0 ? names[0] : string.Empty;
            var last = names.Length > 1 ? string.Join(' ', names.Skip(1)) : string.Empty;

            var user = new AppUser
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = first,
                LastName = last,
                PhoneNumber = request.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, request.Password ?? "");
            if (!result.Succeeded)
                return BadRequest(result.Errors.Select(e => e.Description));

            // Create Member record
            var member = new Member
            {
                UserId = user.Id,
                FullName = request.FullName ?? "",
                Email = request.Email ?? "",
                JoinDate = DateTime.UtcNow,
                IsActive = true
            };
            
            _context.Members.Add(member);
            await _context.SaveChangesAsync();

            // Add Member role by default
            await _userManager.AddToRoleAsync(user, "Member");

            var token = await GenerateJwtToken(user, member.Id);
            var refreshToken = GenerateRefreshToken();

            user.RefreshTokens.Add(new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                JwtId = string.Empty,
                IsUsed = false,
                IsRevoked = false,
                AddedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(7)
            });

            await _userManager.UpdateAsync(user);

            return Ok(new LoginResponse
            {
                AccessToken = token,
                RefreshToken = refreshToken,
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = string.Join(' ', new[] { user.FirstName, user.LastName }.Where(s => !string.IsNullOrEmpty(s)))
                }
            });
        }

        /// <summary>
        /// Refresh access token
        /// </summary>
        [HttpPost("refresh-token")]
        public async Task<ActionResult<RefreshTokenResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.AccessToken); // fallback: user from access token not provided
            if (user == null)
                return Unauthorized("Người dùng không tồn tại");

            var refreshToken = user.RefreshTokens.FirstOrDefault(rt =>
                rt.Token == request.RefreshToken &&
                rt.ExpiryDate > DateTime.UtcNow &&
                !rt.IsRevoked &&
                !rt.IsUsed);

            if (refreshToken == null)
                return Unauthorized("Refresh token không hợp lệ hoặc hết hạn");

            var member = await _context.Members
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.UserId == user.Id);

            var newAccessToken = await GenerateJwtToken(user, member?.Id ?? 0);
            var newRefreshToken = GenerateRefreshToken();

            // mark old token used/revoked
            refreshToken.IsUsed = true;
            refreshToken.IsRevoked = true;

            user.RefreshTokens.Add(new RefreshToken
            {
                UserId = user.Id,
                Token = newRefreshToken,
                JwtId = string.Empty,
                IsUsed = false,
                IsRevoked = false,
                AddedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(7)
            });

            await _userManager.UpdateAsync(user);

            return Ok(new RefreshTokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }

        /// <summary>
        /// Logout (revoke refresh tokens)
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] AuthLogoutRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.Username ?? "");
            if (user == null)
                return NotFound("Người dùng không tồn tại");

            // Mark all refresh tokens as revoked/used
            foreach (var rt in user.RefreshTokens)
            {
                rt.IsRevoked = true;
                rt.IsUsed = true;
            }
            await _userManager.UpdateAsync(user);

            return Ok("Đăng xuất thành công");
        }

        /// <summary>
        /// Get current user profile
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetProfile()
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return NotFound();

            return Ok(new UserDto
            {
                Id = user.Id,
                Email = user.Email ?? "",
                FullName = string.Join(' ', new[] { user.FirstName, user.LastName }.Where(s => !string.IsNullOrEmpty(s)))
            });
        }

        /// <summary>
        /// Generate JWT token
        /// </summary>
        private async Task<string> GenerateJwtToken(AppUser user, int memberId)
        {
            var keyStr = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            if (string.IsNullOrEmpty(keyStr)) throw new ArgumentNullException("Jwt:Key is missing in configuration");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyStr));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserName ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim("UserId", memberId.ToString())
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Generate random refresh token
        /// </summary>
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    /// <summary>
    /// Logout request
    /// </summary>
    public class AuthLogoutRequest
    {
        public string? Username { get; set; }
    }
}
