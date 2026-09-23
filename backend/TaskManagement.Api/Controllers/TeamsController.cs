using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.Data;
using TaskManagement.Api.DTOs;
using TaskManagement.Api.Models;

namespace TaskManagement.Api.Controllers;

[ApiController]
[Route("api/teams")]
[Authorize]
public class TeamsController : ControllerBase
{
    private readonly AppDbContext _db;

    public TeamsController(AppDbContext db)
    {
        _db = db;
    }

    // GET /api/teams - any authenticated user can view teams
    [HttpGet]
    public async Task<ActionResult<List<TeamResponseDto>>> GetTeams()
    {
        var teams = await _db.Teams
            .Include(t => t.Manager)
            .Include(t => t.Members)
            .Select(t => new TeamResponseDto
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                Manager = t.Manager == null ? null : new UserDto
                {
                    Id = t.Manager.Id,
                    Name = t.Manager.Name,
                    Email = t.Manager.Email,
                    Role = t.Manager.Role.ToString(),
                    TeamId = t.Manager.TeamId
                },
                Members = t.Members.Select(m => new UserDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Email = m.Email,
                    Role = m.Role.ToString(),
                    TeamId = m.TeamId
                }).ToList()
            })
            .ToListAsync();

        return Ok(teams);
    }

    // GET /api/teams/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<TeamResponseDto>> GetTeam(int id)
    {
        var team = await _db.Teams
            .Include(t => t.Manager)
            .Include(t => t.Members)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (team is null) return NotFound();

        return Ok(new TeamResponseDto
        {
            Id = team.Id,
            Name = team.Name,
            Description = team.Description,
            Manager = team.Manager == null ? null : new UserDto
            {
                Id = team.Manager.Id,
                Name = team.Manager.Name,
                Email = team.Manager.Email,
                Role = team.Manager.Role.ToString(),
                TeamId = team.Manager.TeamId
            },
            Members = team.Members.Select(m => new UserDto
            {
                Id = m.Id,
                Name = m.Name,
                Email = m.Email,
                Role = m.Role.ToString(),
                TeamId = m.TeamId
            }).ToList()
        });
    }

    // POST /api/teams - Admin only
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<TeamResponseDto>> CreateTeam(CreateTeamDto dto)
    {
        if (dto.ManagerId.HasValue)
        {
            var managerExists = await _db.Users.AnyAsync(u => u.Id == dto.ManagerId.Value);
            if (!managerExists) return BadRequest(new { message = "Manager not found." });
        }

        var team = new Team
        {
            Name = dto.Name,
            Description = dto.Description,
            ManagerId = dto.ManagerId
        };

        _db.Teams.Add(team);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTeam), new { id = team.Id }, new TeamResponseDto
        {
            Id = team.Id,
            Name = team.Name,
            Description = team.Description,
            Members = new List<UserDto>()
        });
    }
}