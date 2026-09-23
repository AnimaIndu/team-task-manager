using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.Data;
using TaskManagement.Api.DTOs;
using TaskManagement.Api.Models;

namespace TaskManagement.Api.Controllers;

[ApiController]
[Route("api/tasks/{taskId}/comments")]
[Authorize]
public class CommentsController : ControllerBase
{
    private readonly AppDbContext _db;

    public CommentsController(AppDbContext db)
    {
        _db = db;
    }

    private int CurrentUserId => int.Parse(User.FindFirst("userId")!.Value);

    // GET /api/tasks/{taskId}/comments
    [HttpGet]
    public async Task<ActionResult<List<CommentResponseDto>>> GetComments(int taskId)
    {
        var taskExists = await _db.Tasks.AnyAsync(t => t.Id == taskId);
        if (!taskExists) return NotFound(new { message = "Task not found." });

        var comments = await _db.Comments
            .Where(c => c.TaskId == taskId)
            .Include(c => c.Author)
            .OrderBy(c => c.CreatedAt)
            .Select(c => new CommentResponseDto
            {
                Id = c.Id,
                Text = c.Text,
                CreatedAt = c.CreatedAt,
                Author = new UserDto
                {
                    Id = c.Author!.Id,
                    Name = c.Author.Name,
                    Email = c.Author.Email,
                    Role = c.Author.Role.ToString(),
                    TeamId = c.Author.TeamId
                }
            })
            .ToListAsync();

        return Ok(comments);
    }

    // POST /api/tasks/{taskId}/comments
    [HttpPost]
    public async Task<ActionResult<CommentResponseDto>> AddComment(int taskId, CreateCommentDto dto)
    {
        var task = await _db.Tasks.FindAsync(taskId);
        if (task is null) return NotFound(new { message = "Task not found." });

        var comment = new Comment
        {
            TaskId = taskId,
            UserId = CurrentUserId,
            Text = dto.Text
        };

        _db.Comments.Add(comment);
        await _db.SaveChangesAsync();

        var author = await _db.Users.FindAsync(CurrentUserId);

        return Ok(new CommentResponseDto
        {
            Id = comment.Id,
            Text = comment.Text,
            CreatedAt = comment.CreatedAt,
            Author = new UserDto
            {
                Id = author!.Id,
                Name = author.Name,
                Email = author.Email,
                Role = author.Role.ToString(),
                TeamId = author.TeamId
            }
        });
    }
}