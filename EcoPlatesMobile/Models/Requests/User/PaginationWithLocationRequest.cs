using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoPlatesMobile.Models.Requests.User
{
    public class PaginationWithLocationRequest
    {
        public int pageSize{ get; set; }
        public int offset{ get; set; }
        public double user_lat{ get; set; }
        public double user_lon { get; set; }
    }
}
