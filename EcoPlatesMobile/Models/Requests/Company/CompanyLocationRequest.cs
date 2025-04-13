using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoPlatesMobile.Models.Requests.Company
{
    public class CompanyLocationRequest
    {
        public int pageSize { get; set; }
        public int offset { get; set; }
        public double user_lat { get; set; }
        public double user_lon { get; set; }
        public double radius_km { get; set; }
    }
}
