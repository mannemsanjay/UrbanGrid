using UrbanGrid.Core.Enums;

namespace UrbanGrid.Core.Entities;

public class User : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public UserRole Role { get; set; } = UserRole.CITIZEN;
    public string Status { get; set; } = "ACTIVE";

    public ICollection<FaultReport> FaultReports { get; set; } = [];
    public ICollection<Notification> Notifications { get; set; } = [];
    public ICollection<AuditLog> AuditLogs { get; set; } = [];
    public bool MustChangePassword { get; set; } = false;

}
