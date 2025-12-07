using SecurityAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace SecurityAPI.DTOs;

public class CreateUserDto
{
    [Required, StringLength(50, MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;

    [Required, StringLength(128, MinimumLength = 8)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public Role Role { get; set; } = Role.User;
}

public class UpdateUserDto
{
    [StringLength(128, MinimumLength = 8)]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    public Role? Role { get; set; }
}

public class UserResponse
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = "User";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}