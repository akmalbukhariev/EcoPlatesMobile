namespace EcoPlatesMobile.Models.User
{
    public class UserInfo
    {
        public double location_latitude { get; set; }
        public string? profile_picture_url { get; set; }
        public string? last_name { get; set; }

        public DateTime created_at => DateTimeOffset.FromUnixTimeMilliseconds(_created_at).UtcDateTime;
        public long _created_at { get; set; }

        public string? token_mb { get; set; }
        public string? full_name { get; set; }

        private string _deleted { get; set; }
        public bool deleted => _deleted.Equals("true", StringComparison.OrdinalIgnoreCase);

        public DateTime updated_at => DateTimeOffset.FromUnixTimeMilliseconds(_updated_at).UtcDateTime;
        public long _updated_at { get; set; }

        public int user_id { get; set; }
        public string phone_number { get; set; }
        public string? first_name { get; set; }
        public double location_longitude { get; set; }
        public string? email { get; set; }
        public string status { get; set; } = "INACTIVE";
    }
}
