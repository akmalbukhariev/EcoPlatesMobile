
using EcoPlatesMobile.Utilities;

namespace EcoPlatesMobile.Models.Requests.User
{
    public class RegisterPosterFeedbackRequest
    {
        public long promotion_id { get; set; }
        public long user_id { get; set; }
        public int rating { get; set; }
        public string feedback_type1 { get; set; }
        public string feedback_type2 { get; set; }
        public string feedback_type3 { get; set; }
        public string feedback_text { get; set; } = "Empty";
    }
}
