using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UrbanGrid.Application.DTOs.Common;
using UrbanGrid.Infrastructure.Data;

namespace UrbanGrid.API.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly UrbanGridDbContext _ctx;

    public NotificationsController(UrbanGridDbContext ctx) => _ctx = ctx;

    [HttpGet]
    public async Task<IActionResult> GetNotifications([FromQuery] string? status = null)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var query = _ctx.Notifications
            .Where(n => n.UserId == userId)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status))
            query = query.Where(n => n.Status == status.ToUpper());

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(n => n.CreatedAt)
            .Take(50)
            .Select(n => new {
                n.Id, n.Message, n.Category, n.Status,
                n.EntityId, n.CreatedAt
            })
            .ToListAsync();

        return Ok(ApiResponse<object>.Success(new
        {
            notifications = items,
            pagination = new { total, page = 1, limit = 50 }
        }));
    }

    [HttpPatch("{id}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var notif = await _ctx.Notifications
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

        if (notif == null) return NotFound();
        notif.Status = "READ";
        await _ctx.SaveChangesAsync();
        return Ok(ApiResponse<object>.Success(null, "Marked as read"));
    }
}
