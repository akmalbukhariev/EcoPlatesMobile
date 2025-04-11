using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EcoPlatesMobile.Models.Requests.User
{
    public class RegisterBookmarkCompanyRequest
    {
        public long user_id { get; set; }
        public long company_id { get; set; }
    }
}
