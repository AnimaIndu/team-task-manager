using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Api.DTOs;

public class CreateTeamDto
{
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int? ManagerId { get; set; }
}

public class TeamResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public UserDto? Manager { get; set; }
    public List<UserDto> Members { get; set; } = new();
}