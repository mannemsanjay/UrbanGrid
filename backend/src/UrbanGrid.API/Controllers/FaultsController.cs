using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UrbanGrid.Application.DTOs.Common;
using UrbanGrid.Application.DTOs.Faults;
using UrbanGrid.Application.Interfaces;
using UrbanGrid.Core.Enums;

namespace UrbanGrid.API.Controllers;

[ApiController]
[Route("api/faults")]
[Authorize]
public class FaultsController : ControllerBase
{
    private readonly IFaultService _faultService;

    public FaultsController(IFaultService faultService) =>
        _faultService = faultService;

    [HttpGet]
    public async Task<IActionResult> GetFaults(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 20,
        [FromQuery] FaultStatus? status = null,
        [FromQuery] Guid? assetId = null)
    {
        var result = await _faultService.GetFaultsAsync(page, limit, status, assetId);
        return Ok(ApiResponse<FaultListResponse>.Success(result));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetFault(Guid id)
    {
        var result = await _faultService.GetByIdAsync(id);
        return Ok(ApiResponse<object>.Success(new { fault = result }));
    }

    [HttpPost]
    public async Task<IActionResult> CreateFault([FromForm] CreateFaultRequest request,
        IFormFile? photo)
    {
        var userId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        string? photoUri = null;

        if (photo != null)
        {
            var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            Directory.CreateDirectory(uploadsDir);
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(photo.FileName)}";
            var filePath = Path.Combine(uploadsDir, fileName);
            using var stream = System.IO.File.Create(filePath);
            await photo.CopyToAsync(stream);
            photoUri = $"/uploads/{fileName}";
        }

        var result = await _faultService.CreateAsync(request, userId, photoUri);
        return CreatedAtAction(nameof(GetFault), new { id = result.Id },
            ApiResponse<FaultDto>.Success(result));
    }

    [HttpPost("{id}/validate")]
    [Authorize(Roles = "DISPATCHER,ADMIN")]
    public async Task<IActionResult> ValidateFault(
        Guid id, [FromBody] ValidateFaultRequest request)
    {
        var userId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _faultService.ValidateAsync(id, request, userId);
        return Ok(ApiResponse<FaultDto>.Success(result));
    }
}
