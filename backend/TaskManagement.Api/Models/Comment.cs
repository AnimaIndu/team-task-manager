namespace TaskManagement.Api.Models;

public class Comment
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;

    public int TaskId { get; set; }
    public TaskItem? Task { get; set; }

    public int UserId { get; set; }
    public User? Author { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}