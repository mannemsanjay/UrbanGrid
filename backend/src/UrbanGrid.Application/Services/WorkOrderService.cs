using UrbanGrid.Application.DTOs.WorkOrders;
using UrbanGrid.Application.Interfaces;
using UrbanGrid.Core.Entities;
using UrbanGrid.Core.Enums;
using UrbanGrid.Core.Interfaces;

namespace UrbanGrid.Application.Services;

public class WorkOrderService : IWorkOrderService
{
    private readonly IWorkOrderRepository _repo;

    public WorkOrderService(IWorkOrderRepository repo) => _repo = repo;

    public async Task<WorkOrderListResponse> GetWorkOrdersAsync(
        int page, int limit, WorkOrderStatus? status, WorkOrderPriority? priority)
    {
        var (items, total) = await _repo.GetAllAsync(page, limit, status, priority);
        return new WorkOrderListResponse
        {
            WorkOrders = items.Select(MapToDto),
            Pagination = new PaginationMeta
            {
                Total = total, Page = page, Limit = limit,
                Pages = (int)Math.Ceiling((double)total / limit)
            }
        };
    }

    public async Task<WorkOrderDto> GetByIdAsync(Guid id)
    {
        var wo = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Work order {id} not found");
        return MapToDto(wo);
    }

    public async Task<WorkOrderDto> CreateAsync(
        CreateWorkOrderRequest request, Guid createdBy)
    {
        var wo = new WorkOrder
        {
            SourceType = request.SourceType,
            SourceId = request.SourceId,
            AssetId = request.AssetId,
            Description = request.Description,
            Priority = request.Priority,
            CreatedBy = createdBy,
            AssignedCrewId = request.AssignedCrewId,
            ScheduledStart = request.ScheduledStart,
            ScheduledEnd = request.ScheduledEnd
        };

        if (request.AssignedCrewId.HasValue)
            wo.Status = WorkOrderStatus.ASSIGNED;

        var created = await _repo.CreateAsync(wo);
        return MapToDto(created);
    }

    public async Task<WorkOrderDto> UpdateStatusAsync(
        Guid id, string status, string? notes)
    {
        var wo = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Work order not found");

        wo.Status = Enum.Parse<WorkOrderStatus>(status, ignoreCase: true);
        wo.UpdatedAt = DateTime.UtcNow;
        var updated = await _repo.UpdateAsync(wo);
        return MapToDto(updated);
    }

    public async Task<WorkOrderDto> AssignCrewAsync(Guid id, Guid crewId)
    {
        var wo = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Work order not found");

        wo.AssignedCrewId = crewId;
        wo.Status = WorkOrderStatus.ASSIGNED;
        wo.UpdatedAt = DateTime.UtcNow;
        var updated = await _repo.UpdateAsync(wo);
        return MapToDto(updated);
    }

    private static WorkOrderDto MapToDto(WorkOrder wo) => new()
    {
        Id = wo.Id, SourceType = wo.SourceType.ToString(),
        SourceId = wo.SourceId, AssetId = wo.AssetId,
        Description = wo.Description, Priority = wo.Priority.ToString(),
        CreatedBy = wo.CreatedBy, CreatedAt = wo.CreatedAt,
        AssignedCrewId = wo.AssignedCrewId,
        ScheduledStart = wo.ScheduledStart, ScheduledEnd = wo.ScheduledEnd,
        Status = wo.Status.ToString(),
        Asset = wo.Asset == null ? null : new AssetSummaryDto
            { Id = wo.Asset.Id, AssetTag = wo.Asset.AssetTag },
        Creator = wo.Creator == null ? null : new CreatorDto
            { Id = wo.Creator.Id, Name = wo.Creator.Name },
        Crew = wo.Crew == null ? null : new CrewDto
            { Id = wo.Crew.Id, Name = wo.Crew.Name }
    };
}
