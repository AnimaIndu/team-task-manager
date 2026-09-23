using System.ComponentModel.DataAnnotations;
using TaskManagement.Api.Models;

namespace TaskManagement.Api.DTOs;

public class CreateTaskDto
{
    [Required, StringLength(200)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    public DateTime? Deadline { get; set; }

    [Required]
    public int AssignedToId { get; set; }

    public int? TeamId { get; set; }
}

public class UpdateTaskStatusDto
{
    [Required]
    public TaskStatusEnum Status { get; set; }
}

public class TaskResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public DateTime? Deadline { get; set; }
    public UserDto? AssignedTo { get; set; }
    public UserDto? AssignedBy { get; set; }
    public int? TeamId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}