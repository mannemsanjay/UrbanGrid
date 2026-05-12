using UrbanGrid.Core.Enums;

namespace UrbanGrid.Application.DTOs.Assets;

public class AssetDto
{
    public Guid Id { get; set; }
    public string AssetTag { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Model { get; set; }
    public string? Manufacturer { get; set; }
    public DateTime? InstallDate { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? LocationDescription { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateAssetRequest
{
    public string AssetTag { get; set; } = string.Empty;
    public AssetType Type { get; set; }
    public string? Model { get; set; }
    public string? Manufacturer { get; set; }
    public DateTime? InstallDate { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? LocationDescription { get; set; }
}

public class UpdateAssetRequest : CreateAssetRequest
{
    public AssetStatus? Status { get; set; }
}

public class AssetListResponse
{
    public IEnumerable<AssetDto> Assets { get; set; } = [];
    public PaginationMeta Pagination { get; set; } = new();
}

public class PaginationMeta
{
    public int Total { get; set; }
    public int Page { get; set; }
    public int Limit { get; set; }
    public int Pages { get; set; }
}
