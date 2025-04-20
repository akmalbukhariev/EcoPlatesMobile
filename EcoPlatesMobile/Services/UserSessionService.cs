
public class UserSessionService
{
    public string? UserId { get; private set; }
    public UserRole Role { get; private set; } = UserRole.None;

    public void SetUser(string userId, UserRole role)
    {
        UserId = userId;
        Role = role;
    }

    public void SetUser(UserRole role)
    {
        Role = role;
    }

    public void Clear()
    {
        UserId = null;
        Role = UserRole.None;
    }

    public bool IsLoggedIn => UserId != null;
}