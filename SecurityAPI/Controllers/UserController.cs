using BCrypt.Net;
using SecurityAPI.Data;
using SecurityAPI.DTOs;
using SecurityAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SecurityAPI.Controllers;

[ApiController]
[Route("users")]
[Authorize] // Require valid access token only (no AdminOnly policy)
public class UsersController(AppDbContext db) : ControllerBase
{
    [HttpGet("me/{username}")]
    public async Task<IActionResult> GetByUsername([FromRoute] string username)
    {
        if (string.IsNullOrWhiteSpace(username)) return BadRequest(new { error = "Username is required" });

        var u = await db.Users.AsNoTracking().SingleOrDefaultAsync(x => x.Username == username);
        if (u is null) return NotFound(new { error = "Not found" });

        return Ok(new UserResponse
        {
            Id = u.Id,
            Username = u.Username,
            Role = u.Role.ToString(),
            CreatedAt = u.CreatedAt,
            UpdatedAt = u.UpdatedAt
        });
    }

    [HttpGet]
    public async Task<IActionResult> ListUsers()
    {
        var data = await db.Users
            .OrderBy(u => u.Id)
            .Select(u => new UserResponse
            {
                Id = u.Id,
                Username = u.Username,
                Role = u.Role.ToString(),
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt
            })
            .ToListAsync();

        return Ok(new { data });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var u = await db.Users.FindAsync(id);
        if (u is null) return NotFound(new { error = "Not found" });

        return Ok(new UserResponse
        {
            Id = u.Id,
            Username = u.Username,
            Role = u.Role.ToString(),
            CreatedAt = u.CreatedAt,
            UpdatedAt = u.UpdatedAt
        });
    }

    [HttpPatch("user/{id:int}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateUserDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var user = await db.Users.FindAsync(id);
        if (user is null) return NotFound(new { error = "Not found" });

        if (!string.IsNullOrWhiteSpace(dto.Password))
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        }
        if (dto.Role.HasValue)
        {
            user.Role = dto.Role.Value;
        }
        user.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return Ok(new UserResponse
        {
            Id = user.Id,
            Username = user.Username,
            Role = user.Role.ToString(),
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        });
    }

    [HttpDelete("user/{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var user = await db.Users.FindAsync(id);
        if (user is null) return NotFound();

        db.Users.Remove(user);
        await db.SaveChangesAsync();
        return NoContent();
    }
}