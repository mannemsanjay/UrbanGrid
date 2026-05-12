using UrbanGrid.Application.DTOs.WorkOrders;
using UrbanGrid.Core.Enums;

namespace UrbanGrid.Application.Interfaces;

public interface IWorkOrderService
{
    Task<WorkOrderListResponse> GetWorkOrdersAsync(
        int page, int limit,
        WorkOrderStatus? status, WorkOrderPriority? priority);
    Task<WorkOrderDto> GetByIdAsync(Guid id);
    Task<WorkOrderDto> CreateAsync(CreateWorkOrderRequest request, Guid createdBy);
    Task<WorkOrderDto> UpdateStatusAsync(Guid id, string status, string? notes);
    Task<WorkOrderDto> AssignCrewAsync(Guid id, Guid crewId);
}
