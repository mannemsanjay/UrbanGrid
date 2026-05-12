using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrbanGrid.Application.DTOs.Assets;
using UrbanGrid.Application.DTOs.Common;
using UrbanGrid.Application.Interfaces;
using UrbanGrid.Core.Enums;

namespace UrbanGrid.API.Controllers;

[ApiController]
[Route("api/assets")]
[Authorize]
public class AssetsController : ControllerBase
{
    private readonly IAssetService _assetService;

    public AssetsController(IAssetService assetService) =>
        _assetService = assetService;

    [HttpGet]
    public async Task<IActionResult> GetAssets(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 20,
        [FromQuery] AssetType? type = null,
        [FromQuery] AssetStatus? status = null,
        [FromQuery] string? search = null)
    {
        var result = await _assetService.GetAssetsAsync(page, limit, type, status, search);
        return Ok(ApiResponse<AssetListResponse>.Success(result));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsset(Guid id)
    {
        var result = await _assetService.GetByIdAsync(id);
        return Ok(ApiResponse<object>.Success(new { asset = result }));
    }

    [HttpPost]
    [Authorize(Roles = "ASSET_MANAGER,ADMIN")]
    public async Task<IActionResult> CreateAsset([FromBody] CreateAssetRequest request)
    {
        var result = await _assetService.CreateAsync(request);
        return CreatedAtAction(nameof(GetAsset), new { id = result.Id },
            ApiResponse<AssetDto>.Success(result));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "ASSET_MANAGER,ADMIN")]
    public async Task<IActionResult> UpdateAsset(
        Guid id, [FromBody] UpdateAssetRequest request)
    {
        var result = await _assetService.UpdateAsync(id, request);
        return Ok(ApiResponse<AssetDto>.Success(result));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> DeleteAsset(Guid id)
    {
        await _assetService.DeleteAsync(id);
        return Ok(ApiResponse<object>.Success(null, "Asset deleted"));
    }
}
