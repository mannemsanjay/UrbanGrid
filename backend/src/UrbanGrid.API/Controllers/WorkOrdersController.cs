using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UrbanGrid.Application.DTOs.Common;
using UrbanGrid.Application.DTOs.WorkOrders;
using UrbanGrid.Application.Interfaces;
using UrbanGrid.Core.Enums;

namespace UrbanGrid.API.Controllers;

[ApiController]
[Route("api/work-orders")]
[Authorize]
public class WorkOrdersController : ControllerBase
{
    private readonly IWorkOrderService _woService;

    public WorkOrdersController(IWorkOrderService woService) =>
        _woService = woService;

    [HttpGet]
    public async Task<IActionResult> GetWorkOrders(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 20,
        [FromQuery] WorkOrderStatus? status = null,
        [FromQuery] WorkOrderPriority? priority = null)
    {
        var result = await _woService.GetWorkOrdersAsync(page, limit, status, priority);
        return Ok(ApiResponse<WorkOrderListResponse>.Success(result));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetWorkOrder(Guid id)
    {
        var result = await _woService.GetByIdAsync(id);
        return Ok(ApiResponse<object>.Success(new { workOrder = result }));
    }

    [HttpPost]
    [Authorize(Roles = "DISPATCHER,ADMIN")]
    public async Task<IActionResult> CreateWorkOrder(
        [FromBody] CreateWorkOrderRequest request)
    {
        var userId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _woService.CreateAsync(request, userId);
        return CreatedAtAction(nameof(GetWorkOrder), new { id = result.Id },
            ApiResponse<WorkOrderDto>.Success(result));
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(
        Guid id, [FromBody] UpdateStatusRequest request)
    {
        var result = await _woService.UpdateStatusAsync(id, request.Status, request.Notes);
        return Ok(ApiResponse<WorkOrderDto>.Success(result));
    }

    [HttpPatch("{id}/assign")]
    [Authorize(Roles = "DISPATCHER,ADMIN")]
    public async Task<IActionResult> AssignCrew(
        Guid id, [FromBody] AssignCrewRequest request)
    {
        var result = await _woService.AssignCrewAsync(id, request.CrewId);
        return Ok(ApiResponse<WorkOrderDto>.Success(result));
    }
}
