using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EcoPlatesMobile.Models.Company
{
    public class PosterInfo
    {
        public long? poster_id { get; set; }
        public long? company_id { get; set; }
        public string company_name { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public decimal? old_price { get; set; }
        public decimal? new_price { get; set; }
        public double? location_latitude { get; set; }
        public double? location_longitude { get; set; }
        public double? distance_km { get; set; }
        public string image_url { get; set; }
        public string category { get; set; }
        public long? views_count { get; set; }
        public long? click_to_contact_count { get; set; }
        public DateTime? end_date { get; set; }
        public DateTime? created_at { get; set; }
    }
}
