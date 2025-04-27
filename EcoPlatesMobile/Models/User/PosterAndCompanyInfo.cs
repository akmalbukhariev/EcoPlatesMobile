using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcoPlatesMobile.Utilities;

namespace EcoPlatesMobile.Models.Responses.User
{
    public class PosterAndCompanyInfo
    {
        public string company_name { get; set; }
        public string phone_number { get; set; }
        public string email { get; set; }
        public string logo_url { get; set; }
        public BusinessType business_type { get; set; }
        public string user_need_to_know { get; set; }
        public double? rating { get; set; }
        public double location_latitude { get; set; }
        public double location_longitude { get; set; }
        public string working_hours { get; set; }
        public string telegram_link { get; set; }
        public string social_profile_link { get; set; }
        public string about { get; set; }

        public long poster_id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public decimal old_price { get; set; }
        public decimal new_price { get; set; }
        public string image_url { get; set; }
        public PosterType category { get; set; }
        public int? quantity_available { get; set; }
        public long? views_count { get; set; }
        public long? click_to_contact_count { get; set; }
        public double? distance_km { get; set; }
        public DateTime? end_date { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public bool deleted { get; set; }
    }
}
