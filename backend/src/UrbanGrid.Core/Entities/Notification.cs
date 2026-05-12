namespace UrbanGrid.Core.Entities;

public class Notification : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid? EntityId { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Status { get; set; } = "UNREAD";
    public User? User { get; set; }
}
