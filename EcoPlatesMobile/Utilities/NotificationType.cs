
public enum NotificationType
{
    PROMOTION,
    NEW_POSTER,
    NEW_MESSAGE,
    FEEDBACK_RESPONSE,
    GENERAL
}

public static class NotificationTypeExtensions
{
    public static string GetValue(this NotificationType type)
    {
        return type.ToString();
    }

    public static NotificationType FromValue(string value)
    {
        if (Enum.TryParse<NotificationType>(value, true, out var result))
        {
            return result;
        }

        throw new ArgumentException($"Unknown NotificationType value: {value}");
    }
}