using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.Data;
using TaskManagement.Api.Models;

namespace TaskManagement.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _db;
    public DashboardController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var userId = int.Parse(User.FindFirst("userId")!.Value);
        var currentUser = await _db.Users.FindAsync(userId);
        if (currentUser == null) return Unauthorized();

        var query = _db.Tasks.AsQueryable();
        if (currentUser.Role == UserRole.User)
            query = query.Where(t => t.AssignedToId == userId);
        else if (currentUser.Role == UserRole.Manager)
            query = query.Where(t => t.TeamId == currentUser.TeamId);
        // Admin sees everything — no filter

        var statusCounts = await query
            .GroupBy(t => t.Status)
            .Select(g => new { status = g.Key.ToString(), count = g.Count() })
            .ToListAsync();

        var priorityCounts = await query
            .GroupBy(t => t.Priority)
            .Select(g => new { priority = g.Key.ToString(), count = g.Count() })
            .ToListAsync();

        var overdueCount = await query.CountAsync(t =>
            t.Deadline != null && t.Deadline < DateTime.UtcNow && t.Status != TaskStatusEnum.Done);

        return Ok(new { statusCounts, priorityCounts, overdueCount });
    }
}