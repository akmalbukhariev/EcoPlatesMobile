using CommunityToolkit.Mvvm.ComponentModel;

namespace EcoPlatesMobile.Models
{
    public partial class Announcement : ObservableObject
    {
        [ObservableProperty] private long announcementId;

        [ObservableProperty] private string title;

        /// <summary>
        /// Short text shown in the list (preview). Could be the first 1–2 lines of Body.
        /// </summary>
        [ObservableProperty] private string preview;

        /// <summary>
        /// Full content for the detail page.
        /// </summary>
        [ObservableProperty] private string body;

        /// <summary>
        /// Optional image/banner URL (if you want announcements with images later).
        /// </summary>
        [ObservableProperty] private string? imageUrl;

        /// <summary>
        /// Server creation time (UTC recommended).
        /// </summary>
        [ObservableProperty] private DateTime createdAtUtc = DateTime.UtcNow;

        /// <summary>
        /// If true, hide unread dot.
        /// </summary>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsUnread))]
        private bool isRead;
        public bool IsUnread => !IsRead;

        /// <summary>
        /// Convenience property for UI binding (what your XAML is using).
        /// You can format it however you want.
        /// </summary>
        [ObservableProperty] private string createdAtText;

        /// <summary>
        /// Optional: if you want announcements to expire.
        /// </summary>
        [ObservableProperty] private DateTime? expiresAtUtc;

        public bool IsExpired => ExpiresAtUtc.HasValue && DateTime.UtcNow > ExpiresAtUtc.Value;

        public Announcement()
        {
            createdAtText = createdAtUtc.ToLocalTime().ToString("MMM dd, yyyy");
        }
    }
}