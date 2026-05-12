using UrbanGrid.Core.Enums;

namespace UrbanGrid.Application.DTOs.WorkOrders;

public class WorkOrderDto
{
    public Guid Id { get; set; }
    public string SourceType { get; set; } = string.Empty;
    public Guid SourceId { get; set; }
    public Guid AssetId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? AssignedCrewId { get; set; }
    public DateTime? ScheduledStart { get; set; }
    public DateTime? ScheduledEnd { get; set; }
    public string Status { get; set; } = string.Empty;
    public AssetSummaryDto? Asset { get; set; }
    public CreatorDto? Creator { get; set; }
    public CrewDto? Crew { get; set; }
}

public class AssetSummaryDto { public Guid Id { get; set; } public string AssetTag { get; set; } = string.Empty; }
public class CreatorDto { public Guid Id { get; set; } public string Name { get; set; } = string.Empty; }
public class CrewDto { public Guid Id { get; set; } public string Name { get; set; } = string.Empty; }

public class CreateWorkOrderRequest
{
    public WorkOrderSourceType SourceType { get; set; }
    public Guid SourceId { get; set; }
    public Guid AssetId { get; set; }
    public string Description { get; set; } = string.Empty;
    public WorkOrderPriority Priority { get; set; } = WorkOrderPriority.MEDIUM;
    public Guid? AssignedCrewId { get; set; }
    public DateTime? ScheduledStart { get; set; }
    public DateTime? ScheduledEnd { get; set; }
}

public class UpdateStatusRequest
{
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class AssignCrewRequest { public Guid CrewId { get; set; } }

public class WorkOrderListResponse
{
    public IEnumerable<WorkOrderDto> WorkOrders { get; set; } = [];
    public PaginationMeta Pagination { get; set; } = new();
}

public class PaginationMeta
{
    public int Total { get; set; }
    public int Page { get; set; }
    public int Limit { get; set; }
    public int Pages { get; set; }
}
