using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoPlatesMobile.Models.Requests
{
    public class PaginationRequest
    {
        public int pageSize { get; set; }
        public int offset { get; set; }
    }
}
