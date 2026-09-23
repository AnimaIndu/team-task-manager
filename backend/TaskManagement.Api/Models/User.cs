namespace TaskManagement.Api.Models;

public enum UserRole { Admin, Manager, User }

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.User;

    public int? TeamId { get; set; }
    public Team? Team { get; set; }

    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}