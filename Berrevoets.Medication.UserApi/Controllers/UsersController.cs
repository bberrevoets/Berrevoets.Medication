using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Berrevoets.Medication.UserApi.Controllers.Requests;
using Berrevoets.Medication.UserApi.Data;
using Berrevoets.Medication.UserApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Berrevoets.Medication.UserApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly UserDbContext _context;
    private readonly PasswordHasher<User> _passwordHasher;

    public UsersController(UserDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
        _passwordHasher = new PasswordHasher<User>();
    }

    // POST api/users/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        // Check if the username is already taken
        var existingUser =
            await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username || u.Email == request.Email);
        if (existingUser != null) return BadRequest("User already exists");

        // Create new user and hash the password
        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            CreatedDate = DateTime.UtcNow,
            LastUpdateDate = DateTime.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "User registered successfully" });
    }

    // POST api/users/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // Find the user by username

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
        if (user == null) return Unauthorized("Invalid username or password");

        // Verify the password
        var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verificationResult == PasswordVerificationResult.Failed)
            return Unauthorized("Invalid username or password");

        // Generate a JWT token for the user
        var token = GenerateJwtToken(user);

        return Ok(new { Token = token });
    }

    // GET api/users/profile
    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        // Get the username from the token claims
        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (username == null) return Unauthorized();

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null) return Unauthorized();

        return Ok(new
            { user.Id, user.Username, user.Email, user.PhoneNumber, user.CreatedDate, user.LastUpdateDate, user.Role });
    }

    // PUT api/users/profile
    [Authorize]
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        // Extract the user id from the token claims.
        var userIdClaim = User.FindFirst("id")?.Value;
        if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();
        if (!Guid.TryParse(userIdClaim, out var userId)) return Unauthorized();

        // Retrieve the user by Id (Guid) rather than by username.
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return Unauthorized();

        // Check uniqueness for new username if it is provided and changed.
        if (!string.IsNullOrWhiteSpace(request.Username) &&
            !request.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase))
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (existingUser != null)
            {
                ModelState.AddModelError("Username", "This username is already taken.");
                return BadRequest(ModelState);
            }

            user.Username = request.Username;
        }

        // Check uniqueness for new email if provided and changed.
        if (!string.IsNullOrWhiteSpace(request.Email) &&
            !request.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase))
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "This email is already in use.");
                return BadRequest(ModelState);
            }

            user.Email = request.Email;
        }

        // Update phone number (if provided)
        user.PhoneNumber = request.PhoneNumber;
        user.LastUpdateDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return Ok(new { Message = "Profile updated successfully" });
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "YourSuperSecretKeyThatIsAtLeast32CharsLong"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim("id", user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}