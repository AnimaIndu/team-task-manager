using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.Models;

namespace TaskManagement.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email).IsUnique();

        modelBuilder.Entity<User>()
            .HasOne(u => u.Team)
            .WithMany(t => t.Members)
            .HasForeignKey(u => u.TeamId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Team>()
            .HasOne(t => t.Manager)
            .WithMany()
            .HasForeignKey(t => t.ManagerId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<TaskItem>()
            .HasOne(t => t.AssignedTo)
            .WithMany()
            .HasForeignKey(t => t.AssignedToId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TaskItem>()
            .HasOne(t => t.AssignedBy)
            .WithMany()
            .HasForeignKey(t => t.AssignedById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TaskItem>()
            .HasOne(t => t.Team)
            .WithMany(team => team.Tasks)
            .HasForeignKey(t => t.TeamId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Task)
            .WithMany(t => t.Comments)
            .HasForeignKey(c => c.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Author)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.User)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}