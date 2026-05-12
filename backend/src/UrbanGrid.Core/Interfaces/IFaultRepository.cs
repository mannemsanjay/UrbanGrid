using UrbanGrid.Core.Entities;
using UrbanGrid.Core.Enums;

namespace UrbanGrid.Core.Interfaces;

public interface IFaultRepository
{
    Task<(IEnumerable<FaultReport> Items, int Total)> GetAllAsync(
        int page, int limit, FaultStatus? status = null, Guid? assetId = null);
    Task<FaultReport?> GetByIdAsync(Guid id);
    Task<FaultReport> CreateAsync(FaultReport fault);
    Task<FaultReport> UpdateAsync(FaultReport fault);
    Task<ValidationNote> AddNoteAsync(ValidationNote note);
    Task<int> CountByStatusAsync(FaultStatus status);
}
