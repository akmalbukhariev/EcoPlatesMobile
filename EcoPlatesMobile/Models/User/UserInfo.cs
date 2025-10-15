using EcoPlatesMobile.Converters;
using EcoPlatesMobile.Utilities;
using Newtonsoft.Json;

namespace EcoPlatesMobile.Models.User
{
    public class UserInfo
    {
        public int user_id { get; set; }
        public double location_latitude { get; set; }
        public string profile_picture_url { get; set; } = string.Empty;
        public string last_name { get; set; } = string.Empty;
        public string token_mb { get; set; } = string.Empty;
        public string full_name { get; set; } = string.Empty;

        [JsonProperty("deleted")]
        private string _deleted { get; set; } = "false";

        [JsonIgnore]
        public bool deleted => _deleted.Equals("true", StringComparison.OrdinalIgnoreCase);
        public string token_frb { get; set; }
        public string phone_number { get; set; } = string.Empty;
        public string first_name { get; set; } = string.Empty;
        public double location_longitude { get; set; }
        public int radius_km { get; set; }
        public int max_radius_km { get; set; }
        
        public string email { get; set; } = string.Empty;
        public string status { get; set; } = string.Empty;
        public string share_link { get; set; } = string.Empty;
        public bool notification_enabled { get; set; }

        [JsonProperty("created_at")]
        [JsonConverter(typeof(FlexibleDateTimeConverter))]
        public DateTime created_at { get; set; }

        [JsonProperty("updated_at")]
        [JsonConverter(typeof(FlexibleDateTimeConverter))]
        public DateTime updated_at { get; set; }

        [JsonProperty("blocked_until")]
        [JsonConverter(typeof(FlexibleDateTimeConverter))]
        public DateTime blocked_until { get; set; }
        public int violation_count { get; set; }
    }
}
