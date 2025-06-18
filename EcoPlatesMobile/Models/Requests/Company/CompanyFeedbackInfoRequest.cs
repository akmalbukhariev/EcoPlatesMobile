using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoPlatesMobile.Models.Requests.Company
{
    public class CompanyFeedbackInfoRequest
    {
        public int company_id { get; set; }
        public string feedback_text { get; set; }
        public string feedback_type { get; set; }
        public int rating { get; set; }
    }
}
