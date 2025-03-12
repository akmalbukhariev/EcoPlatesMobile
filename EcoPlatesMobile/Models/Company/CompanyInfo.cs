using EcoPlatesMobile.Utilities;

namespace EcoPlatesMobile.Models.Company
{
    public class CompanyInfo
    {
        public long company_id { get; set; } = 0;                      
        public string company_name { get; set; } = string.Empty;
        public string phone_number { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string logo_url { get; set; } = string.Empty;                         
        public int rating { get; set; } = 0;
        public double location_latitude { get; set; } = 0;
        public double location_longitude { get; set; } = 0;
        public double distance_km { get; set; } = 0;
        public string working_hours { get; set; } = string.Empty;
        public string telegram_link { get; set; } = string.Empty;                    
        public string social_profile_link { get; set; } = string.Empty;
        public string token_mb { get; set; } = string.Empty;
        public UserOrCompanyStatus status { get; set; } = UserOrCompanyStatus.INACTIVE;
        private string about { get; set; } = string.Empty;
        public DateTime created_at => DateTimeOffset.FromUnixTimeMilliseconds(_created_at).UtcDateTime;
        public long _created_at { get; set; }
        public DateTime updated_at => DateTimeOffset.FromUnixTimeMilliseconds(_updated_at).UtcDateTime;
        public long _updated_at { get; set; }
        public bool deleted { get; set; } = false;
    }
}
