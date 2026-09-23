namespace TaskManagement.Api.Models;

public class Notification
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public string Type { get; set; } = string.Empty; // TASK_ASSIGNED, TASK_STATUS_UPDATED, TASK_COMMENT_ADDED
    public string Message { get; set; } = string.Empty;
    public bool Read { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}