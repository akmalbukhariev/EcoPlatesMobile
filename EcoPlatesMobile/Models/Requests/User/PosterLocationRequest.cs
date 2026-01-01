
using EcoPlatesMobile.Utilities;

namespace EcoPlatesMobile.Models.Requests.User
{
    public class PosterLocationRequest
    {
        public int pageSize { get; set; }
        public int offset { get; set; }
        public double user_lat { get; set; }
        public double user_lon { get; set; }
        public double radius_km { get; set; }
        public string business_type { get; set; }
        public string poster_type { get; set; }
        public string poster_sort { get; set; } = PosterSort.NEAR.GetValue();
    }
}
