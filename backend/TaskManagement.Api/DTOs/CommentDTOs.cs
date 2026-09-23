using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Api.DTOs;

public class CreateCommentDto
{
    [Required, StringLength(1000)]
    public string Text { get; set; } = string.Empty;
}

public class CommentResponseDto
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public UserDto Author { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}