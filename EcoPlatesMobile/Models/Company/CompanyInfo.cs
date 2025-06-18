using EcoPlatesMobile.Converters;
using EcoPlatesMobile.Utilities;
using Newtonsoft.Json;

namespace EcoPlatesMobile.Models.Company
{
    public class CompanyInfo
    {
        public int company_id { get; set; } = 0;                      
        public string company_name { get; set; } = string.Empty;
        public string phone_number { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string logo_url { get; set; } = string.Empty;                         
        public double? rating { get; set; } = 0;
        public double? location_latitude { get; set; } = 0;
        public double? location_longitude { get; set; } = 0;
        public double? distance_km { get; set; } = 0;
        public string business_type { get; set; }
        public string working_hours { get; set; } = string.Empty;
        public string telegram_link { get; set; } = string.Empty;                    
        public string social_profile_link { get; set; } = string.Empty;
        public string token_mb { get; set; } = string.Empty;
        public long active_products { get; set; }
        public long non_active_products { get; set; }
        public UserOrCompanyStatus status { get; set; } = UserOrCompanyStatus.INACTIVE;
        private string about { get; set; } = string.Empty;
         
        [JsonProperty("created_at")]
        [JsonConverter(typeof(FlexibleDateTimeConverter))]
        public DateTime created_at { get; set; }

        [JsonProperty("updated_at")]
        [JsonConverter(typeof(FlexibleDateTimeConverter))]
        public DateTime updated_at { get; set; }

        public long? bookmark_id { get; set; } = 0;
        public bool liked { get; set; }
        public bool deleted { get; set; } = false;
    }
}
