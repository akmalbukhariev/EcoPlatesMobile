
using EcoPlatesMobile.Models.Company;

namespace EcoPlatesMobile.Models.Responses.Company
{
    public class PendingPosterInfo
    {
        public int total { get; set; }
        public List<PosterInfo> data { get; set; }
    }
}