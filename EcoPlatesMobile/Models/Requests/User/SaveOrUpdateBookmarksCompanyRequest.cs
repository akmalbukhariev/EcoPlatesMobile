
namespace EcoPlatesMobile.Models.Requests.User
{
    public class SaveOrUpdateBookmarksCompanyRequest
    {
        public long user_id { get; set; }
        public long company_id { get; set; }
        public bool deleted{ get; set; }
    }
}