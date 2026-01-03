namespace EcoPlatesMobile.Models.Responses.Announcement
{
    public class AnnouncementInfo
    {
        public string preview { get; set; } = string.Empty;
        public int is_read { get; set; }
        public long created_at_utc { get; set; }
        public string image_url { get; set; } = string.Empty;
        public long announcement_id { get; set; }
        public string title { get; set; } = string.Empty;
        public string body { get; set; } = string.Empty;
    }
}