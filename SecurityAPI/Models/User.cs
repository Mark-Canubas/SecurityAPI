using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SecurityAPI.Models;

public enum Role
{
    Admin = 1,
    User = 2
}

[Index(nameof(Username), IsUnique = true)]
public class User
{
    public int Id { get; set; }

    [Required, StringLength(50, MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    public Role Role { get; set; } = Role.User;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}