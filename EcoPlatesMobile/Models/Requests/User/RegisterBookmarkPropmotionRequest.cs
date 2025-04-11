using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoPlatesMobile.Models.Requests.User
{
    public class RegisterBookmarkPropmotionRequest
    {
        public long user_id { get; set; }
        public long promotion_id { get; set; }
    }
}
