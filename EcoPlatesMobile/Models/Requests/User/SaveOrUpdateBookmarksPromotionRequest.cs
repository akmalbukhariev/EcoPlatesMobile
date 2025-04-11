using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EcoPlatesMobile.Models.Requests.User
{
    public class SaveOrUpdateBookmarksPromotionRequest
    {
        public long user_id { get; set; }
        public long promotion_id { get; set; }
        public bool deleted{ get; set; }
    }
}
