

using System.Runtime.InteropServices;

namespace EcoPlatesMobile.Models.Company
{
    public class PosterInfo
    {
        public long? poster_id { get; set; }
        public long? company_id { get; set; }
        public string company_name { get; set; } = string.Empty;
        public string title { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public decimal old_price { get; set; } = 0;
        public decimal new_price { get; set; } = 0;
        public double location_latitude { get; set; } = 0;
        public double location_longitude { get; set; } = 0;
        public double distance_km { get; set; } = 0;
        public string image_url { get; set; } = string.Empty;
        public string category { get; set; } = string.Empty;
        public long views_count { get; set; } = 0;
        public long click_to_contact_count { get; set; } = 0;
        public bool liked { get; set; }
        public long? bookmark_id { get; set; } = 0;
        public DateTime? end_date { get; set; }
        public DateTime? created_at { get; set; }
    }
}
