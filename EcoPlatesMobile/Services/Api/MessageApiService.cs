

using EcoPlatesMobile.Models.Requests;
using EcoPlatesMobile.Models.Responses;
using EcoPlatesMobile.Utilities;
using Newtonsoft.Json;
using RestSharp;

namespace EcoPlatesMobile.Services.Api
{
    public class MessageApiService : ApiService
    {
        private const string BASE_URL = "";
        //private const string BASE_URL = "ecoplatesmessage/api/v1/message/";

        private const string VERIFY_NUMBER = $"{BASE_URL}verifyPhoneNumber";

        public MessageApiService(RestClient client) : base(client)
        {

        }
        
        public async Task<VerifyPhoneNumberResponse> VerifyNumber(VerifyPhoneNumberRequest data)
        {
            var response = new VerifyPhoneNumberResponse();

            try
            {
                var receivedData = await PostAsync(VERIFY_NUMBER, data);

                if (!string.IsNullOrWhiteSpace(receivedData))
                {
                    var deserializedResponse = JsonConvert.DeserializeObject<VerifyPhoneNumberResponse>(receivedData);
                    if (deserializedResponse != null)
                    {
                        return deserializedResponse;
                    }
                }

                response.resultMsg = ApiResult.API_SERVICE_ERROR.GetMessage();
            }
            catch (JsonException jsonEx)
            {
                response.resultCode = ApiResult.JSON_PARSING_ERROR.GetCodeToString();
                response.resultMsg = $"JSON Parsing Error: {jsonEx.Message}";
            }
            catch (Exception ex)
            {
                response.resultCode = ApiResult.API_SERVICE_ERROR.GetCodeToString();
                response.resultMsg = $"API: {ex.Message}";
            }

            return response;
        }
    }
}