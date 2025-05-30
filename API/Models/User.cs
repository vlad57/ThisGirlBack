using System;

namespace API.Models;

public class User
{
    public Guid Id { get; set; }
    public required string Username { get; set; } = string.Empty;
    public required string Email { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime createdAt { get; set; }
    public DateTime? updatedAt { get; set; }
    public bool IsEmailConfirmed { get; set; } = false;
    public string? EmailConfirmationToken { get; set; }
    public DateTime? EmailConfirmationTokenExpiry { get; set; }
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
