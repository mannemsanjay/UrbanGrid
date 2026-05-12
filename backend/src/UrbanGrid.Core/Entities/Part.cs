namespace UrbanGrid.Core.Entities;

public class Part : BaseEntity
{
    public string PartNumber { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int QuantityOnHand { get; set; }
    public int ReorderLevel { get; set; }
    public decimal UnitCost { get; set; }
    public string Status { get; set; } = "ACTIVE";
    public ICollection<MaterialUsage> MaterialUsages { get; set; } = [];
}

public class Supplier : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? ContactInfo { get; set; }
    public string? TaxId { get; set; }
    public string Status { get; set; } = "ACTIVE";
}

public class PurchaseOrder : BaseEntity
{
    public Guid SupplierId { get; set; }
    public string ItemsJson { get; set; } = "[]";
    public decimal TotalAmount { get; set; }
    public Guid CreatedBy { get; set; }
    public string Status { get; set; } = "PENDING";
    public Supplier? Supplier { get; set; }
}
