using EcoPlatesMobile.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoPlatesMobile.Models.Requests.Company
{
    public class CompanyRegisterRequest
    {
        private String company_name{ get; set; }
        private String phone_number { get; set; }
        private String email { get; set; }
        private String logo_url { get; set; }
        private BusinessType business_type { get; set; }
        private String about { get; set; }
        private Stream? logo_data { get; set; }
        private Double location_latitude { get; set; }
        private Double location_longitude { get; set; }
        private String working_hours{ get; set; }
        private String telegram_link { get; set; }
        private String social_profile_link { get; set; }
    }
}
