
namespace EcoPlatesMobile.Models.Responses.User
{
    public class PosterRatingCompanyInfo : PosterAndCompanyInfo
    {
        public PosterRatingInfo ratingInfo { get; set; }
        public List<PosterTypeInfo> typeInfo { get; set; }
    }
}
