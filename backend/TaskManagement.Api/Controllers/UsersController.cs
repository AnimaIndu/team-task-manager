using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.Data;
using TaskManagement.Api.DTOs;
using TaskManagement.Api.Models;

namespace TaskManagement.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _db;

    public UsersController(AppDbContext db)
    {
        _db = db;
    }

    // GET /api/users - list all users (any authenticated user can see names for assignment dropdowns)
    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetUsers()
    {
        var users = await _db.Users
            .Select(u => new UserDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Role = u.Role.ToString(),
                TeamId = u.TeamId
            })
            .ToListAsync();

        return Ok(users);
    }

    // GET /api/users/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user is null) return NotFound();

        return Ok(new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role.ToString(),
            TeamId = user.TeamId
        });
    }

    // PATCH /api/users/{id}/team - Admin assigns a user to a team
    [Authorize(Roles = "Admin")]
    [HttpPatch("{id}/team")]
    public async Task<ActionResult<UserDto>> AssignTeam(int id, [FromBody] AssignTeamDto dto)
    {
        var user = await _db.Users.FindAsync(id);
        if (user is null) return NotFound();

        if (dto.TeamId.HasValue)
        {
            var teamExists = await _db.Teams.AnyAsync(t => t.Id == dto.TeamId.Value);
            if (!teamExists) return BadRequest(new { message = "Team not found." });
        }

        user.TeamId = dto.TeamId;
        await _db.SaveChangesAsync();

        return Ok(new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role.ToString(),
            TeamId = user.TeamId
        });
    }
}