using EcoPlatesMobile.Utilities;
using Newtonsoft.Json;

namespace EcoPlatesMobile.Models.User
{
    public class UserInfo
    {
        public double location_latitude { get; set; }
        public string profile_picture_url { get; set; } = string.Empty;
        public string last_name { get; set; } = string.Empty;

        [JsonIgnore]
        public DateTime created_at => DateTimeOffset.FromUnixTimeMilliseconds(_created_at).UtcDateTime;

        [JsonProperty("created_at")]
        public long _created_at { get; set; }

        public string token_mb { get; set; } = string.Empty;
        public string full_name { get; set; } = string.Empty;

        [JsonProperty("deleted")]
        private string _deleted { get; set; } = "false";

        [JsonIgnore]
        public bool deleted => _deleted.Equals("true", StringComparison.OrdinalIgnoreCase);

        [JsonIgnore]
        public DateTime updated_at => DateTimeOffset.FromUnixTimeMilliseconds(_updated_at).UtcDateTime;

        [JsonProperty("updated_at")]
        public long _updated_at { get; set; }

        public int user_id { get; set; }
        public string phone_number { get; set; } = string.Empty;
        public string first_name { get; set; } = string.Empty;
        public double location_longitude { get; set; }
        public string email { get; set; } = string.Empty;
        public string status { get; set; } = string.Empty;
    }
}
