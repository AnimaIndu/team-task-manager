using TaskManagement.Api.Models;

namespace TaskManagement.Api.Data;

public static class SeedData
{
    public static void Seed(AppDbContext db)
    {
        if (db.Users.Any()) return; // already seeded — skip if you've been testing manually

        var pw = BCrypt.Net.BCrypt.HashPassword("password123");
        var admin = new User { Name = "Alice Admin", Email = "admin@tms.local", PasswordHash = pw, Role = UserRole.Admin };
        var manager = new User { Name = "Mark Manager", Email = "manager@tms.local", PasswordHash = pw, Role = UserRole.Manager };
        var user1 = new User { Name = "Uma User", Email = "user1@tms.local", PasswordHash = pw, Role = UserRole.User };
        var user2 = new User { Name = "Umesh User", Email = "user2@tms.local", PasswordHash = pw, Role = UserRole.User };
        db.Users.AddRange(admin, manager, user1, user2);
        db.SaveChanges();

        var team = new Team { Name = "Engineering", Description = "Product engineering team", ManagerId = manager.Id };
        db.Teams.Add(team);
        db.SaveChanges();

        manager.TeamId = team.Id; user1.TeamId = team.Id; user2.TeamId = team.Id;
        db.SaveChanges();

        db.Tasks.Add(new TaskItem
        {
            Title = "Set up CI pipeline", Description = "Configure GitHub Actions",
            Priority = TaskPriority.High, Deadline = DateTime.UtcNow.AddDays(7),
            AssignedToId = user1.Id, AssignedById = manager.Id, TeamId = team.Id
        });
        db.SaveChanges();
    }
}