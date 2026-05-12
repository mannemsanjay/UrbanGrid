using UrbanGrid.Core.Enums;

namespace UrbanGrid.Core.Entities;

public class FaultReport : BaseEntity
{
    public Guid ReportedBy { get; set; }
    public Guid AssetId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? PhotoUri { get; set; }
    public DateTime ReportedAt { get; set; } = DateTime.UtcNow;
    public Guid? ValidatedBy { get; set; }
    public DateTime? ValidatedAt { get; set; }
    public FaultStatus Status { get; set; } = FaultStatus.REPORTED;

    public User? Reporter { get; set; }
    public Asset? Asset { get; set; }
    public ICollection<ValidationNote> ValidationNotes { get; set; } = [];
}

public class ValidationNote : BaseEntity
{
    public Guid FaultId { get; set; }
    public Guid ValidatorId { get; set; }
    public string NoteText { get; set; } = string.Empty;
    public FaultReport? FaultReport { get; set; }
}
