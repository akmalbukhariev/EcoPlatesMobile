

using EcoPlatesMobile.Utilities;

namespace EcoPlatesMobile.Models.Responses
{
    public class Response<ResultData> where ResultData : class, new()
    {
        public string? resultCode {  get; set; }
        public string? resultMsg { get; set; }
        public ResultData resultData { get; set; }
        public string? apiVersion { get; set; }        
        public string? webVersion { get; set; }

        public Response()
        {
            resultCode = Result.FAILED.GetCodeToString();
            resultMsg = Result.FAILED.GetMessage();
            resultData = new ResultData();
            apiVersion = "";
            webVersion = "";
        }
    }

    public class Response
    {
        public string? resultCode { get; set; }
        public string? resultMsg { get; set; }
        public string? apiVersion { get; set; }
        public string? webVersion { get; set; }
    }
}
