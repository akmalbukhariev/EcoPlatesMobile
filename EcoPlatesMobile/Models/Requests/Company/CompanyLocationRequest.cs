using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoPlatesMobile.Models.Requests.Company
{
    public class CompanyLocationRequest
    {
        private double user_lat { get; set; }
        private double user_lon { get; set; }
        private double radius_km { get; set; }
    }
}
