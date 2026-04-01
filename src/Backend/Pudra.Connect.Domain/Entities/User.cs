using Pudra.Connect.Domain.Enums;

namespace Pudra.Connect.Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public Role Role { get; set; }
    public UserStatus Status { get; set; } = UserStatus.Approved;
    public int ScanCount { get; set; } = 0;
    public string? ProfileImageUrl { get; set; } // YENİ - Base64 veya URL
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}