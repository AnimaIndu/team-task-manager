namespace TaskManagement.Api.Models;

public enum TaskStatusEnum { ToDo, InProgress, Done }
public enum TaskPriority { Low, Medium, High }

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskStatusEnum Status { get; set; } = TaskStatusEnum.ToDo;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public DateTime? Deadline { get; set; }

    public int AssignedToId { get; set; }
    public User? AssignedTo { get; set; }

    public int AssignedById { get; set; }
    public User? AssignedBy { get; set; }

    public int? TeamId { get; set; }
    public Team? Team { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}