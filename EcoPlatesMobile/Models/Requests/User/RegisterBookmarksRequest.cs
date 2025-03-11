using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EcoPlatesMobile.Models.Requests.User
{
    public class RegisterBookmarksRequest
    {
        private long user_id { get; set; }
        private long company_id { get; set; }
    }
}
