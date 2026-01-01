namespace EcoPlatesMobile.Models
{
    public class Announcement
    {
        public long AnnouncementId { get; set; }

        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Short text shown in the list (preview). Could be the first 1–2 lines of Body.
        /// </summary>
        public string Preview { get; set; } = string.Empty;

        /// <summary>
        /// Full content for the detail page.
        /// </summary>
        public string Body { get; set; } = string.Empty;

        /// <summary>
        /// Optional image/banner URL (if you want announcements with images later).
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Server creation time (UTC recommended).
        /// </summary>
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// If true, hide unread dot.
        /// </summary>
        public bool IsRead { get; set; }

        /// <summary>
        /// Convenience property for UI binding (what your XAML is using).
        /// You can format it however you want.
        /// </summary>
        public string CreatedAtText => CreatedAtUtc.ToLocalTime().ToString("MMM dd, yyyy");

        /// <summary>
        /// Optional: if you want announcements to expire.
        /// </summary>
        public DateTime? ExpiresAtUtc { get; set; }

        public bool IsExpired => ExpiresAtUtc.HasValue && DateTime.UtcNow > ExpiresAtUtc.Value;
        public bool IsUnread => !IsRead;
    }
}