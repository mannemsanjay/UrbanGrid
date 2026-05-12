using UrbanGrid.Core.Entities;
using UrbanGrid.Core.Enums;

namespace UrbanGrid.Core.Interfaces;

public interface IWorkOrderRepository
{
    Task<(IEnumerable<WorkOrder> Items, int Total)> GetAllAsync(
        int page, int limit,
        WorkOrderStatus? status = null,
        WorkOrderPriority? priority = null,
        Guid? crewId = null);
    Task<WorkOrder?> GetByIdAsync(Guid id);
    Task<WorkOrder> CreateAsync(WorkOrder workOrder);
    Task<WorkOrder> UpdateAsync(WorkOrder workOrder);
    Task<WorkLog> AddWorkLogAsync(WorkLog log);
    Task<MaterialUsage> AddMaterialUsageAsync(MaterialUsage usage);
    Task<int> CountByStatusAsync(WorkOrderStatus status);
    Task<int> CountCompletedTodayAsync();
}
