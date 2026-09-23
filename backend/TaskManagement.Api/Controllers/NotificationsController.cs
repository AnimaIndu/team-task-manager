using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.Data;

namespace TaskManagement.Api.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly AppDbContext _db;

    public NotificationsController(AppDbContext db)
    {
        _db = db;
    }

    private int CurrentUserId => int.Parse(User.FindFirst("userId")!.Value);

    // GET /api/notifications
    [HttpGet]
    public async Task<ActionResult> GetNotifications()
    {
        var notifications = await _db.Notifications
            .Where(n => n.UserId == CurrentUserId)
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new
            {
                n.Id,
                n.Type,
                n.Message,
                n.Read,
                n.CreatedAt
            })
            .ToListAsync();

        return Ok(notifications);
    }

    // PATCH /api/notifications/{id}/read
    [HttpPatch("{id}/read")]
    public async Task<ActionResult> MarkRead(int id)
    {
        var notification = await _db.Notifications.FindAsync(id);

        if (notification is null || notification.UserId != CurrentUserId)
            return NotFound();

        notification.Read = true;
        await _db.SaveChangesAsync();

        return NoContent();
    }
}