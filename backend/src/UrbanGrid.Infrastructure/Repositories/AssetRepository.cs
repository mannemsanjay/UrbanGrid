using Microsoft.EntityFrameworkCore;
using UrbanGrid.Core.Entities;
using UrbanGrid.Core.Enums;
using UrbanGrid.Core.Interfaces;
using UrbanGrid.Infrastructure.Data;

namespace UrbanGrid.Infrastructure.Repositories;

public class AssetRepository : IAssetRepository
{
    private readonly UrbanGridDbContext _ctx;
    public AssetRepository(UrbanGridDbContext ctx) => _ctx = ctx;

    public async Task<(IEnumerable<Asset> Items, int Total)> GetAllAsync(
        int page, int limit,
        AssetType? type, AssetStatus? status, string? search)
    {
        var query = _ctx.Assets.AsQueryable();

        if (type.HasValue)   query = query.Where(a => a.Type == type.Value);
        if (status.HasValue) query = query.Where(a => a.Status == status.Value);
        if (!string.IsNullOrEmpty(search))
            query = query.Where(a =>
                a.AssetTag.Contains(search) ||
                (a.LocationDescription != null && a.LocationDescription.Contains(search)));

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();

        return (items, total);
    }

    public async Task<Asset?> GetByIdAsync(Guid id) =>
        await _ctx.Assets.FindAsync(id);

    public async Task<Asset?> GetByTagAsync(string tag) =>
        await _ctx.Assets.FirstOrDefaultAsync(a => a.AssetTag == tag);

    public async Task<Asset> CreateAsync(Asset asset)
    {
        _ctx.Assets.Add(asset);
        await _ctx.SaveChangesAsync();
        return asset;
    }

    public async Task<Asset> UpdateAsync(Asset asset)
    {
        _ctx.Assets.Update(asset);
        await _ctx.SaveChangesAsync();
        return asset;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var asset = await _ctx.Assets.FindAsync(id);
        if (asset == null) return false;
        _ctx.Assets.Remove(asset);
        await _ctx.SaveChangesAsync();
        return true;
    }
}
