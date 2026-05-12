using UrbanGrid.Application.Interfaces;
using UrbanGrid.Core.Enums;
using UrbanGrid.Core.Interfaces;

namespace UrbanGrid.Application.Services;

public class DashboardStats
{
    public int TotalAssets { get; set; }
    public int ActiveFaults { get; set; }
    public int OpenWorkOrders { get; set; }
    public int CompletedToday { get; set; }
    public int PendingInspections { get; set; }
    public int LowStockParts { get; set; }
}

public interface IDashboardService
{
    Task<DashboardStats> GetStatsAsync();
}

public class DashboardService : IDashboardService
{
    private readonly IAssetRepository _assetRepo;
    private readonly IFaultRepository _faultRepo;
    private readonly IWorkOrderRepository _workOrderRepo;

    public DashboardService(
        IAssetRepository assetRepo,
        IFaultRepository faultRepo,
        IWorkOrderRepository workOrderRepo)
    {
        _assetRepo = assetRepo;
        _faultRepo = faultRepo;
        _workOrderRepo = workOrderRepo;
    }

    public async Task<DashboardStats> GetStatsAsync()
    {
        var (_, totalAssets) = await _assetRepo.GetAllAsync(1, 1);
        var activeFaults = await _faultRepo.CountByStatusAsync(FaultStatus.REPORTED);
        var openWO = await _workOrderRepo.CountByStatusAsync(WorkOrderStatus.PENDING)
                   + await _workOrderRepo.CountByStatusAsync(WorkOrderStatus.ASSIGNED)
                   + await _workOrderRepo.CountByStatusAsync(WorkOrderStatus.IN_PROGRESS);
        var completedToday = await _workOrderRepo.CountCompletedTodayAsync();

        return new DashboardStats
        {
            TotalAssets = totalAssets,
            ActiveFaults = activeFaults,
            OpenWorkOrders = openWO,
            CompletedToday = completedToday,
            PendingInspections = 0,
            LowStockParts = 0
        };
    }
}
