using UrbanGrid.Application.DTOs.Assets;
using UrbanGrid.Application.Interfaces;
using UrbanGrid.Core.Entities;
using UrbanGrid.Core.Enums;
using UrbanGrid.Core.Interfaces;

namespace UrbanGrid.Application.Services;

public class AssetService : IAssetService
{
    private readonly IAssetRepository _repo;

    public AssetService(IAssetRepository repo) => _repo = repo;

    public async Task<AssetListResponse> GetAssetsAsync(
        int page, int limit,
        AssetType? type, AssetStatus? status, string? search)
    {
        var (items, total) = await _repo.GetAllAsync(page, limit, type, status, search);
        return new AssetListResponse
        {
            Assets = items.Select(MapToDto),
            Pagination = new PaginationMeta
            {
                Total = total, Page = page, Limit = limit,
                Pages = (int)Math.Ceiling((double)total / limit)
            }
        };
    }

    public async Task<AssetDto> GetByIdAsync(Guid id)
    {
        var asset = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Asset {id} not found");
        return MapToDto(asset);
    }

    public async Task<AssetDto> CreateAsync(CreateAssetRequest request)
    {
        var existing = await _repo.GetByTagAsync(request.AssetTag);
        if (existing != null)
            throw new InvalidOperationException("Asset tag already exists");

        var asset = new Asset
        {
            AssetTag = request.AssetTag,
            Type = request.Type,
            Model = request.Model,
            Manufacturer = request.Manufacturer,
            InstallDate = request.InstallDate,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            LocationDescription = request.LocationDescription
        };
        var created = await _repo.CreateAsync(asset);
        return MapToDto(created);
    }

    public async Task<AssetDto> UpdateAsync(Guid id, UpdateAssetRequest request)
    {
        var asset = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Asset not found");

        asset.AssetTag = request.AssetTag;
        asset.Type = request.Type;
        asset.Model = request.Model;
        asset.Manufacturer = request.Manufacturer;
        asset.InstallDate = request.InstallDate;
        asset.Latitude = request.Latitude;
        asset.Longitude = request.Longitude;
        asset.LocationDescription = request.LocationDescription;
        if (request.Status.HasValue) asset.Status = request.Status.Value;
        asset.UpdatedAt = DateTime.UtcNow;

        var updated = await _repo.UpdateAsync(asset);
        return MapToDto(updated);
    }

    public async Task DeleteAsync(Guid id)
    {
        var result = await _repo.DeleteAsync(id);
        if (!result) throw new KeyNotFoundException("Asset not found");
    }

    private static AssetDto MapToDto(Asset a) => new()
    {
        Id = a.Id, AssetTag = a.AssetTag,
        Type = a.Type.ToString(), Model = a.Model,
        Manufacturer = a.Manufacturer, InstallDate = a.InstallDate,
        Latitude = a.Latitude, Longitude = a.Longitude,
        LocationDescription = a.LocationDescription,
        Status = a.Status.ToString(),
        CreatedAt = a.CreatedAt, UpdatedAt = a.UpdatedAt
    };
}
