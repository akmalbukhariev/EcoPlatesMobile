using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcoPlatesMobile.Models.Company;

namespace EcoPlatesMobile.Services
{
    public class AppControl
    {
        public bool ShowCompanyMoreInfo { get; set; } = true;
        public CompanyInfo CompanyInfo{ get; set; }
        public Dictionary<string, string> businessTypeList = new Dictionary<string, string>
        {
            { "Restaurant", "RESTAURANT" },
            { "Bakery", "BAKERY" },
            { "Fast Food", "FAST_FOOD" },
            { "Cafe", "CAFE" },
            { "Supermarket", "SUPERMARKET" },
            { "Other", "OTHER" }
        };
    }
}
