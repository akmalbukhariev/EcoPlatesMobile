using EcoPlatesMobile.Models.Requests.Chat;
using EcoPlatesMobile.Models.Responses.Chat;
using EcoPlatesMobile.Utilities;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoPlatesMobile.Services.Api
{
    public class ChatApiService : ApiService
    {
        //private const string BASE_URL = "";
        private const string BASE_URL = "ecoplateschatting/api/v1/";
        private const string GET_MESSAGE_HISTORY = $"{BASE_URL}chat/getChatHistory";
        private const string GET_SENDER_ID_LIST_WITH_UNREAD_INFO = $"{BASE_URL}chat/getSendersWithUnread";

        public ChatApiService(RestClient client) : base(client)
        {

        }

        public async Task<ChatSenderIdResponse> GetSendersWithUnread(UnreadMessagesRequest data)
        {
            var response = new ChatSenderIdResponse();

            try
            {
                var receivedData = await PostAsync(GET_SENDER_ID_LIST_WITH_UNREAD_INFO, data);

                if (!string.IsNullOrWhiteSpace(receivedData))
                {
                    var deserializedResponse = JsonConvert.DeserializeObject<ChatSenderIdResponse>(receivedData);
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
                response.resultMsg = $"Login Error: {ex.Message}";
            }

            return response;
        }

        public async Task<ChatMessageResponse> GetHistoryMessage(ChatMessageRequest data)
        {
            var response = new ChatMessageResponse();

            try
            {
                var receivedData = await PostAsync(GET_MESSAGE_HISTORY, data);

                if (!string.IsNullOrWhiteSpace(receivedData))
                {
                    var deserializedResponse = JsonConvert.DeserializeObject<ChatMessageResponse>(receivedData);
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
