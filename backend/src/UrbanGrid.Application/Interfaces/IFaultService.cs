using UrbanGrid.Application.DTOs.Faults;
using UrbanGrid.Core.Enums;

namespace UrbanGrid.Application.Interfaces;

public interface IFaultService
{
    Task<FaultListResponse> GetFaultsAsync(int page, int limit, FaultStatus? status, Guid? assetId);
    Task<FaultDto> GetByIdAsync(Guid id);
    Task<FaultDto> CreateAsync(CreateFaultRequest request, Guid reportedBy, string? photoUri);
    Task<FaultDto> ValidateAsync(Guid id, ValidateFaultRequest request, Guid validatorId);
}
