namespace UrbanGrid.Core.Entities;

public class Inspection : BaseEntity
{
    public Guid AssetId { get; set; }
    public Guid InspectorId { get; set; }
    public DateTime InspectionDate { get; set; }
    public int ConditionRating { get; set; }
    public string? Findings { get; set; }
    public string? PhotoUri { get; set; }
    public string Status { get; set; } = "PENDING";
    public Asset? Asset { get; set; }
    public User? Inspector { get; set; }
}

public class InspectionPlan : BaseEntity
{
    public string AssetFilterJson { get; set; } = "{}";
    public int FrequencyDays { get; set; }
    public DateTime NextDueDate { get; set; }
    public Guid OwnerId { get; set; }
    public string Status { get; set; } = "ACTIVE";
}
