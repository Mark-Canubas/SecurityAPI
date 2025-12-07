using SecurityAPI.Data;
using SecurityAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecurityAPI.DTOs;

namespace SecurityAPI.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(AppDbContext db) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var exists = await db.Users.AnyAsync(u => u.Username == dto.Username);
        if (exists) return Conflict(new { error = "Username already taken" });

        var user = new User
        {
            Username = dto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = Role.User,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        return Ok(new
        {
            Status = "Creation Successful",
        });

    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var user = await db.Users.SingleOrDefaultAsync(x => x.Username == dto.Username);
        if (user is null) return Unauthorized(new { error = "Invalid credentials" });

        var ok = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
        if (!ok) return Unauthorized(new { error = "Invalid credentials" });

        return Ok(new
        {
            Status = "Login Successful",
        });
    }
}