  
namespace EcoPlatesMobile.Models.Responses.User
{
    public class PosterInfoByCompanyResponse
    {
        public int? poster_id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public double? old_price { get; set; }
        public double? new_price { get; set; }
        public string image_url { get; set; }
        public PosterType category { get; set; }
        public double avg_rating { get; set; }
        public int total_reviews { get; set; }
    }
}
