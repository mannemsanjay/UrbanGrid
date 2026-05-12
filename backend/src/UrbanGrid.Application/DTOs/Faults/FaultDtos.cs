using UrbanGrid.Core.Enums;

namespace UrbanGrid.Application.DTOs.Faults;

public class FaultDto
{
    public Guid Id { get; set; }
    public Guid ReportedBy { get; set; }
    public Guid AssetId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? PhotoUri { get; set; }
    public DateTime ReportedAt { get; set; }
    public Guid? ValidatedBy { get; set; }
    public DateTime? ValidatedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public ReporterDto? Reporter { get; set; }
    public AssetSummaryDto? Asset { get; set; }
}

public class ReporterDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class AssetSummaryDto
{
    public Guid Id { get; set; }
    public string AssetTag { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? LocationDescription { get; set; }
}

public class CreateFaultRequest
{
    public Guid AssetId { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class ValidateFaultRequest
{
    public string NoteText { get; set; } = string.Empty;
    public string Action { get; set; } = "VALIDATE"; // VALIDATE or REJECT
}

public class FaultListResponse
{
    public IEnumerable<FaultDto> Faults { get; set; } = [];
    public PaginationMeta Pagination { get; set; } = new();
}

public class PaginationMeta
{
    public int Total { get; set; }
    public int Page { get; set; }
    public int Limit { get; set; }
    public int Pages { get; set; }
}
