using EcoPlatesMobile.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoPlatesMobile.Models.Requests.User
{
    public class UserFeedbackInfoRequest
    {
        public int user_id { get; set; }
        public string feedback_text { get; set; }
        public string feedback_type { get; set; }
        public int rating { get; set; }
    }
}
