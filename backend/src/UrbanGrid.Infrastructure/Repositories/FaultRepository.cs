using Microsoft.EntityFrameworkCore;
using UrbanGrid.Core.Entities;
using UrbanGrid.Core.Enums;
using UrbanGrid.Core.Interfaces;
using UrbanGrid.Infrastructure.Data;

namespace UrbanGrid.Infrastructure.Repositories;

public class FaultRepository : IFaultRepository
{
    private readonly UrbanGridDbContext _ctx;
    public FaultRepository(UrbanGridDbContext ctx) => _ctx = ctx;

    public async Task<(IEnumerable<FaultReport> Items, int Total)> GetAllAsync(
        int page, int limit, FaultStatus? status, Guid? assetId)
    {
        var query = _ctx.FaultReports
            .Include(f => f.Reporter)
            .Include(f => f.Asset)
            .AsQueryable();

        if (status.HasValue)  query = query.Where(f => f.Status == status.Value);
        if (assetId.HasValue) query = query.Where(f => f.AssetId == assetId.Value);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(f => f.ReportedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();

        return (items, total);
    }

    public async Task<FaultReport?> GetByIdAsync(Guid id) =>
        await _ctx.FaultReports
            .Include(f => f.Reporter)
            .Include(f => f.Asset)
            .Include(f => f.ValidationNotes)
            .FirstOrDefaultAsync(f => f.Id == id);

    public async Task<FaultReport> CreateAsync(FaultReport fault)
    {
        _ctx.FaultReports.Add(fault);
        await _ctx.SaveChangesAsync();
        return fault;
    }

    public async Task<FaultReport> UpdateAsync(FaultReport fault)
    {
        _ctx.FaultReports.Update(fault);
        await _ctx.SaveChangesAsync();
        return fault;
    }

    public async Task<ValidationNote> AddNoteAsync(ValidationNote note)
    {
        _ctx.ValidationNotes.Add(note);
        await _ctx.SaveChangesAsync();
        return note;
    }

    public async Task<int> CountByStatusAsync(FaultStatus status) =>
        await _ctx.FaultReports.CountAsync(f => f.Status == status);
}
