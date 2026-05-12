using UrbanGrid.Core.Enums;

namespace UrbanGrid.Core.Entities;

public class Asset : BaseEntity
{
    public string AssetTag { get; set; } = string.Empty;
    public AssetType Type { get; set; }
    public string? Model { get; set; }
    public string? Manufacturer { get; set; }
    public DateTime? InstallDate { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? LocationDescription { get; set; }
    public AssetStatus Status { get; set; } = AssetStatus.ACTIVE;

    public ICollection<FaultReport> FaultReports { get; set; } = [];
    public ICollection<WorkOrder> WorkOrders { get; set; } = [];
    public ICollection<Inspection> Inspections { get; set; } = [];
}
