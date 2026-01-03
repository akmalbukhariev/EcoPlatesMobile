
namespace EcoPlatesMobile.Models.Requests
{
    public class MarkAnnouncementReadRequest
    {
        public int announcementId { get; set; }
        public string actorType { get; set; }
        public int actorId { get; set; }
    }
}