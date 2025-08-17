
namespace EcoPlatesMobile.Models.Requests.Company
{
    public class ChangePosterDeletionListRequest
    {
        public List<ChangePosterDeletionRequest> dataList { get; set; }

        public ChangePosterDeletionListRequest()
        {
            dataList = new List<ChangePosterDeletionRequest>();
        }
    }
}