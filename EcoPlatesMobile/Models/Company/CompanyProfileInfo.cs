using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoPlatesMobile.Models.Company
{
    public class CompanyProfileInfo
    {
        public String company_name { get; set; }
        public String phone_number { get; set; }
        public String logo_url { get; set; }
        public long active_products { get; set; }
        public long non_active_products { get; set; }
    }
}
