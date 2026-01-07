using CommunityToolkit.Mvvm.ComponentModel;

namespace EcoPlatesMobile.Models
{
     public partial class Announcement : ObservableObject
    {
        [ObservableProperty]
        private long announcementId;

        [ObservableProperty]
        private string title = string.Empty;

        [ObservableProperty]
        private string preview = string.Empty;

        [ObservableProperty]
        private string body = string.Empty;

        [ObservableProperty]
        private string? imageUrl;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CreatedAtText))]
        private DateTime createdAtUtc;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsUnread))]
        private bool isRead;

        public bool IsUnread => !IsRead;

        /// <summary>
        /// UI-friendly formatted creation date
        /// </summary>
        public string CreatedAtText =>
            CreatedAtUtc == default
                ? string.Empty
                : CreatedAtUtc.ToLocalTime().ToString("yyyy.MM.dd");

        [ObservableProperty]
        private DateTime? expiresAtUtc;

        public bool IsExpired =>
            ExpiresAtUtc.HasValue && DateTime.UtcNow > ExpiresAtUtc.Value;
    }
}