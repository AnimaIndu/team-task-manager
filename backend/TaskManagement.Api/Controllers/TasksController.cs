using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.Data;
using TaskManagement.Api.DTOs;
using TaskManagement.Api.Models;

namespace TaskManagement.Api.Controllers;

[ApiController]
[Route("api/tasks")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly AppDbContext _db;

    public TasksController(AppDbContext db)
    {
        _db = db;
    }

    private int CurrentUserId => int.Parse(User.FindFirst("userId")!.Value);
    private string CurrentUserRole => User.FindFirst(System.Security.Claims.ClaimTypes.Role)!.Value;

    // GET /api/tasks?status=&priority=
    [HttpGet]
    public async Task<ActionResult<List<TaskResponseDto>>> GetTasks(
        [FromQuery] TaskStatusEnum? status,
        [FromQuery] TaskPriority? priority)
    {
        var query = _db.Tasks
            .Include(t => t.AssignedTo)
            .Include(t => t.AssignedBy)
            .AsQueryable();

        // Role-based scoping
        if (CurrentUserRole == "User")
        {
            query = query.Where(t => t.AssignedToId == CurrentUserId);
        }
        else if (CurrentUserRole == "Manager")
        {
            var managedTeamIds = await _db.Teams
                .Where(team => team.ManagerId == CurrentUserId)
                .Select(team => team.Id)
                .ToListAsync();

            query = query.Where(t => t.TeamId != null && managedTeamIds.Contains(t.TeamId.Value));
        }
        // Admin: no filter, sees everything

        if (status.HasValue)
            query = query.Where(t => t.Status == status.Value);

        if (priority.HasValue)
            query = query.Where(t => t.Priority == priority.Value);

        var tasks = await query
            .Select(t => new TaskResponseDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status.ToString(),
                Priority = t.Priority.ToString(),
                Deadline = t.Deadline,
                AssignedTo = t.AssignedTo == null ? null : new UserDto
                {
                    Id = t.AssignedTo.Id,
                    Name = t.AssignedTo.Name,
                    Email = t.AssignedTo.Email,
                    Role = t.AssignedTo.Role.ToString(),
                    TeamId = t.AssignedTo.TeamId
                },
                AssignedBy = t.AssignedBy == null ? null : new UserDto
                {
                    Id = t.AssignedBy.Id,
                    Name = t.AssignedBy.Name,
                    Email = t.AssignedBy.Email,
                    Role = t.AssignedBy.Role.ToString(),
                    TeamId = t.AssignedBy.TeamId
                },
                TeamId = t.TeamId,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            })
            .ToListAsync();

        return Ok(tasks);
    }

    // GET /api/tasks/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<TaskResponseDto>> GetTask(int id)
    {
        var task = await _db.Tasks
            .Include(t => t.AssignedTo)
            .Include(t => t.AssignedBy)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task is null) return NotFound();

        if (!await CanAccessTask(task)) return Forbid();

        return Ok(ToResponseDto(task));
    }

    // POST /api/tasks - Admin or Manager only
    [Authorize(Roles = "Admin,Manager")]
    [HttpPost]
    public async Task<ActionResult<TaskResponseDto>> CreateTask(CreateTaskDto dto)
    {
        var assignee = await _db.Users.FindAsync(dto.AssignedToId);
        if (assignee is null) return BadRequest(new { message = "Assignee not found." });

        var task = new TaskItem
        {
            Title = dto.Title,
            Description = dto.Description,
            Priority = dto.Priority,
            Deadline = dto.Deadline,
            AssignedToId = dto.AssignedToId,
            AssignedById = CurrentUserId,
            TeamId = dto.TeamId
        };

        _db.Tasks.Add(task);

        // Notification: task assignment (mock)
        var notification = new Notification
        {
            UserId = dto.AssignedToId,
            Type = "TASK_ASSIGNED",
            Message = $"You have been assigned a new task: \"{task.Title}\""
        };
        _db.Notifications.Add(notification);

        await _db.SaveChangesAsync();

        Console.WriteLine($"[MOCK EMAIL] To: {assignee.Email} | Subject: New Task Assigned | Task: {task.Title}");

        var created = await _db.Tasks
            .Include(t => t.AssignedTo)
            .Include(t => t.AssignedBy)
            .FirstAsync(t => t.Id == task.Id);

        return CreatedAtAction(nameof(GetTask), new { id = task.Id }, ToResponseDto(created));
    }

    // PUT /api/tasks/{id} - update status
    [HttpPut("{id}")]
    public async Task<ActionResult<TaskResponseDto>> UpdateStatus(int id, UpdateTaskStatusDto dto)
    {
        var task = await _db.Tasks
            .Include(t => t.AssignedTo)
            .Include(t => t.AssignedBy)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task is null) return NotFound();

        if (!await CanAccessTask(task)) return Forbid();

        task.Status = dto.Status;
        task.UpdatedAt = DateTime.UtcNow;

        // Notification: status update (mock)
        var notification = new Notification
        {
            UserId = task.AssignedById,
            Type = "TASK_STATUS_UPDATED",
            Message = $"Task \"{task.Title}\" status changed to {task.Status}"
        };
        _db.Notifications.Add(notification);

        await _db.SaveChangesAsync();

        Console.WriteLine($"[MOCK EMAIL] Task \"{task.Title}\" status updated to {task.Status}");

        return Ok(ToResponseDto(task));
    }

    private async Task<bool> CanAccessTask(TaskItem task)
    {
        if (CurrentUserRole == "Admin") return true;

        if (CurrentUserRole == "User")
            return task.AssignedToId == CurrentUserId;

        if (CurrentUserRole == "Manager")
        {
            if (task.TeamId is null) return false;
            return await _db.Teams.AnyAsync(t => t.Id == task.TeamId && t.ManagerId == CurrentUserId);
        }

        return false;
    }

    private static TaskResponseDto ToResponseDto(TaskItem t) => new()
    {
        Id = t.Id,
        Title = t.Title,
        Description = t.Description,
        Status = t.Status.ToString(),
        Priority = t.Priority.ToString(),
        Deadline = t.Deadline,
        AssignedTo = t.AssignedTo == null ? null : new UserDto
        {
            Id = t.AssignedTo.Id,
            Name = t.AssignedTo.Name,
            Email = t.AssignedTo.Email,
            Role = t.AssignedTo.Role.ToString(),
            TeamId = t.AssignedTo.TeamId
        },
        AssignedBy = t.AssignedBy == null ? null : new UserDto
        {
            Id = t.AssignedBy.Id,
            Name = t.AssignedBy.Name,
            Email = t.AssignedBy.Email,
            Role = t.AssignedBy.Role.ToString(),
            TeamId = t.AssignedBy.TeamId
        },
        TeamId = t.TeamId,
        CreatedAt = t.CreatedAt,
        UpdatedAt = t.UpdatedAt
    };
}