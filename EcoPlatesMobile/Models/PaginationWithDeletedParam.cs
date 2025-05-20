namespace EcoPlatesMobile.Models.Requests
{
    public class PaginationWithDeletedParam
    {
        public bool deleted{ get; set; }
        public int pageSize { get; set; }
        public int offset { get; set; }
    }
}