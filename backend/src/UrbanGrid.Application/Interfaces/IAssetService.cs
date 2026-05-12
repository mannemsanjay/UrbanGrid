using UrbanGrid.Application.DTOs.Assets;
using UrbanGrid.Core.Enums;

namespace UrbanGrid.Application.Interfaces;

public interface IAssetService
{
    Task<AssetListResponse> GetAssetsAsync(
        int page, int limit,
        AssetType? type = null,
        AssetStatus? status = null,
        string? search = null);
    Task<AssetDto> GetByIdAsync(Guid id);
    Task<AssetDto> CreateAsync(CreateAssetRequest request);
    Task<AssetDto> UpdateAsync(Guid id, UpdateAssetRequest request);
    Task DeleteAsync(Guid id);
}
