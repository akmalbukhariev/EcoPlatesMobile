using EcoPlatesMobile.Utilities;

namespace EcoPlatesMobile.Models.Requests.Company
{
    public class UpdateCompanyInfoRequest
    {
        private string? company_name { get; set; }
        private string? phone_number { get; set; }
        private string? email { get; set; }
        private string? logo_url { get; set; }
        private BusinessType business_type { get; set; }
        private string? about { get; set; }
        private Stream? logo_data { get; set; }
        private double location_latitude { get; set; }
        private double location_longitude { get; set; }
        private string? working_hours { get; set; }
        private string? telegram_link { get; set; }
        private string? social_profile_link { get; set; }
    }
}
