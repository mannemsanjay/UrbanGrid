using UrbanGrid.Core.Enums;

namespace UrbanGrid.Core.Entities;

public class WorkOrder : BaseEntity
{
    public WorkOrderSourceType SourceType { get; set; }
    public Guid SourceId { get; set; }
    public Guid AssetId { get; set; }
    public string Description { get; set; } = string.Empty;
    public WorkOrderPriority Priority { get; set; } = WorkOrderPriority.MEDIUM;
    public Guid CreatedBy { get; set; }
    public Guid? AssignedCrewId { get; set; }
    public DateTime? ScheduledStart { get; set; }
    public DateTime? ScheduledEnd { get; set; }
    public WorkOrderStatus Status { get; set; } = WorkOrderStatus.PENDING;

    public Asset? Asset { get; set; }
    public User? Creator { get; set; }
    public Crew? Crew { get; set; }
    public ICollection<WorkLog> WorkLogs { get; set; } = [];
    public ICollection<MaterialUsage> MaterialUsages { get; set; } = [];
}

public class Crew : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string MembersJson { get; set; } = "[]";
    public string? ContactInfo { get; set; }
    public string Status { get; set; } = "ACTIVE";
    public ICollection<WorkOrder> WorkOrders { get; set; } = [];
}

public class WorkLog : BaseEntity
{
    public Guid WorkOrderId { get; set; }
    public Guid PerformedBy { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public double LaborHours { get; set; }
    public string? Notes { get; set; }
    public string? PhotoUri { get; set; }
    public WorkOrder? WorkOrder { get; set; }
}

public class MaterialUsage : BaseEntity
{
    public Guid WorkOrderId { get; set; }
    public Guid PartId { get; set; }
    public int QuantityUsed { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalCost { get; set; }
    public WorkOrder? WorkOrder { get; set; }
    public Part? Part { get; set; }
}
