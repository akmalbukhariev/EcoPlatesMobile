
using EcoPlatesMobile.Utilities;

namespace EcoPlatesMobile.Models.Requests
{
    public class GetAnnouncementsRequest : PaginationRequest
    {
        public string actorType { get; set; }
        public int actorId;
    }
}