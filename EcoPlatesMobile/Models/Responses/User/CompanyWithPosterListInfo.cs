  
namespace EcoPlatesMobile.Models.Responses.User
{
    public class CompanyWithPosterListInfo
    {
        private int company_id { get; set; }
        public string company_name { get; set; }
        public string phone_number { get; set; }
        public string business_type { get; set; }
        public string logo_url { get; set; }
        public double location_latitude { get; set; }
        public double location_longitude { get; set; }
        public bool liked { get; set; }
        public string working_hours { get; set; }
        public List<PosterInfoByCompanyResponse> posterList { get; set; }
    }
}
