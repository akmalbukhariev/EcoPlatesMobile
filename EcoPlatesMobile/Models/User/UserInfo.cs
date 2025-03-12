using EcoPlatesMobile.Utilities;

namespace EcoPlatesMobile.Models.User
{
    public class UserInfo
    {
        public double location_latitude { get; set; } = 0;
        public string profile_picture_url { get; set; } = string.Empty;
        public string last_name { get; set; } = string.Empty;

        public DateTime created_at => DateTimeOffset.FromUnixTimeMilliseconds(_created_at).UtcDateTime;
        public long _created_at { get; set; } = 0;

        public string token_mb { get; set; } = string.Empty;
        public string full_name { get; set; } = string.Empty;

        private string _deleted { get; set; } = "false";
        public bool deleted => _deleted.Equals("true", StringComparison.OrdinalIgnoreCase);

        public DateTime updated_at => DateTimeOffset.FromUnixTimeMilliseconds(_updated_at).UtcDateTime;
        public long _updated_at { get; set; } = 0;

        public int user_id { get; set; } = 0;
        public string phone_number { get; set; } = string.Empty;
        public string first_name { get; set; } = string.Empty;
        public double location_longitude { get; set; } = 0;
        public string email { get; set; } = string.Empty;
        public string status { get; set; } = UserOrCompanyStatus.INACTIVE.GetValue();
    }
}
