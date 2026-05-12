using UrbanGrid.Core.Entities;
using UrbanGrid.Core.Enums;

namespace UrbanGrid.Core.Interfaces;

public interface IAssetRepository
{
    Task<(IEnumerable<Asset> Items, int Total)> GetAllAsync(
        int page, int limit,
        AssetType? type = null,
        AssetStatus? status = null,
        string? search = null);
    Task<Asset?> GetByIdAsync(Guid id);
    Task<Asset?> GetByTagAsync(string tag);
    Task<Asset> CreateAsync(Asset asset);
    Task<Asset> UpdateAsync(Asset asset);
    Task<bool> DeleteAsync(Guid id);
}
