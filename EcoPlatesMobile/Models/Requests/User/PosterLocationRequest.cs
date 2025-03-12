using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoPlatesMobile.Models.Requests.User
{
    public class PosterLocationRequest
    {
        private int pageSize { get; set; }
        private int offset { get; set; }
        private double user_lat{ get; set; }
        private double user_lon { get; set; }
        private double radius_km { get; set; }
        private PosterType category { get; set; }
    }
}
