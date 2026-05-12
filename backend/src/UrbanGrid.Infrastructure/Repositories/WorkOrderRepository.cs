using Microsoft.EntityFrameworkCore;
using UrbanGrid.Core.Entities;
using UrbanGrid.Core.Enums;
using UrbanGrid.Core.Interfaces;
using UrbanGrid.Infrastructure.Data;

namespace UrbanGrid.Infrastructure.Repositories;

public class WorkOrderRepository : IWorkOrderRepository
{
    private readonly UrbanGridDbContext _ctx;
    public WorkOrderRepository(UrbanGridDbContext ctx) => _ctx = ctx;

    public async Task<(IEnumerable<WorkOrder> Items, int Total)> GetAllAsync(
        int page, int limit,
        WorkOrderStatus? status, WorkOrderPriority? priority, Guid? crewId = null)
    {
        var query = _ctx.WorkOrders
            .Include(w => w.Asset)
            .Include(w => w.Creator)
            .Include(w => w.Crew)
            .AsQueryable();

        if (status.HasValue)   query = query.Where(w => w.Status == status.Value);
        if (priority.HasValue) query = query.Where(w => w.Priority == priority.Value);
        if (crewId.HasValue)   query = query.Where(w => w.AssignedCrewId == crewId.Value);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(w => w.CreatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();

        return (items, total);
    }

    public async Task<WorkOrder?> GetByIdAsync(Guid id) =>
        await _ctx.WorkOrders
            .Include(w => w.Asset)
            .Include(w => w.Creator)
            .Include(w => w.Crew)
            .Include(w => w.WorkLogs)
            .Include(w => w.MaterialUsages).ThenInclude(m => m.Part)
            .FirstOrDefaultAsync(w => w.Id == id);

    public async Task<WorkOrder> CreateAsync(WorkOrder wo)
    {
        _ctx.WorkOrders.Add(wo);
        await _ctx.SaveChangesAsync();
        return wo;
    }

    public async Task<WorkOrder> UpdateAsync(WorkOrder wo)
    {
        _ctx.WorkOrders.Update(wo);
        await _ctx.SaveChangesAsync();
        return wo;
    }

    public async Task<WorkLog> AddWorkLogAsync(WorkLog log)
    {
        _ctx.WorkLogs.Add(log);
        await _ctx.SaveChangesAsync();
        return log;
    }

    public async Task<MaterialUsage> AddMaterialUsageAsync(MaterialUsage usage)
    {
        _ctx.MaterialUsages.Add(usage);
        await _ctx.SaveChangesAsync();
        return usage;
    }

    public async Task<int> CountByStatusAsync(WorkOrderStatus status) =>
        await _ctx.WorkOrders.CountAsync(w => w.Status == status);

    public async Task<int> CountCompletedTodayAsync()
    {
        var today = DateTime.UtcNow.Date;
        return await _ctx.WorkOrders.CountAsync(
            w => w.Status == WorkOrderStatus.COMPLETED &&
                 w.UpdatedAt >= today);
    }
}
