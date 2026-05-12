using UrbanGrid.Application.DTOs.Faults;
using UrbanGrid.Application.Interfaces;
using UrbanGrid.Core.Entities;
using UrbanGrid.Core.Enums;
using UrbanGrid.Core.Interfaces;

namespace UrbanGrid.Application.Services;

public class FaultService : IFaultService
{
    private readonly IFaultRepository _repo;
    private readonly IAssetRepository _assetRepo;

    public FaultService(IFaultRepository repo, IAssetRepository assetRepo)
    {
        _repo = repo;
        _assetRepo = assetRepo;
    }

    public async Task<FaultListResponse> GetFaultsAsync(
        int page, int limit, FaultStatus? status, Guid? assetId)
    {
        var (items, total) = await _repo.GetAllAsync(page, limit, status, assetId);
        return new FaultListResponse
        {
            Faults = items.Select(MapToDto),
            Pagination = new PaginationMeta
            {
                Total = total, Page = page, Limit = limit,
                Pages = (int)Math.Ceiling((double)total / limit)
            }
        };
    }

    public async Task<FaultDto> GetByIdAsync(Guid id)
    {
        var fault = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Fault {id} not found");
        return MapToDto(fault);
    }

    public async Task<FaultDto> CreateAsync(
        CreateFaultRequest request, Guid reportedBy, string? photoUri)
    {
        var asset = await _assetRepo.GetByIdAsync(request.AssetId)
            ?? throw new KeyNotFoundException("Asset not found");

        var fault = new FaultReport
        {
            AssetId = request.AssetId,
            ReportedBy = reportedBy,
            Description = request.Description,
            PhotoUri = photoUri
        };

        asset.Status = AssetStatus.FAULTY;
        await _assetRepo.UpdateAsync(asset);

        var created = await _repo.CreateAsync(fault);
        return MapToDto(created);
    }

    public async Task<FaultDto> ValidateAsync(
        Guid id, ValidateFaultRequest request, Guid validatorId)
    {
        var fault = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Fault not found");

        fault.Status = request.Action == "VALIDATE"
            ? FaultStatus.VALIDATED : FaultStatus.REJECTED;
        fault.ValidatedBy = validatorId;
        fault.ValidatedAt = DateTime.UtcNow;

        await _repo.AddNoteAsync(new ValidationNote
        {
            FaultId = id,
            ValidatorId = validatorId,
            NoteText = request.NoteText
        });

        var updated = await _repo.UpdateAsync(fault);
        return MapToDto(updated);
    }

    private static FaultDto MapToDto(FaultReport f) => new()
    {
        Id = f.Id, ReportedBy = f.ReportedBy, AssetId = f.AssetId,
        Description = f.Description, PhotoUri = f.PhotoUri,
        ReportedAt = f.ReportedAt, ValidatedBy = f.ValidatedBy,
        ValidatedAt = f.ValidatedAt, Status = f.Status.ToString(),
        Reporter = f.Reporter == null ? null : new ReporterDto
        {
            Id = f.Reporter.Id, Name = f.Reporter.Name, Email = f.Reporter.Email
        },
        Asset = f.Asset == null ? null : new AssetSummaryDto
        {
            Id = f.Asset.Id, AssetTag = f.Asset.AssetTag,
            Type = f.Asset.Type.ToString(),
            LocationDescription = f.Asset.LocationDescription
        }
    };
}
