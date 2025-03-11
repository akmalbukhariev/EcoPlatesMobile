using EcoPlatesMobile.Models.Requests;
using EcoPlatesMobile.Models.Responses.User;
using EcoPlatesMobile.Utilities;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoPlatesMobile.Services
{
    public class UserApiService : ApiService
    {
        private const string LOGIN_USER = "login";
        private const string LOGOUT_USER = "logout";
        private const string REGISTER_USER = "register";
        private const string GET_USER_INFO = "getUserInfo";
        private const string UPDATE_USER_INFO = "updateUserInfo";
        private const string REGISTER_BOOKMARK = "registerBookmark";
        private const string GET_USER_BOOKMARK = "getUserBookmark";
        private const string GET_COMPANIES_BY_USER_LOCATION = "getCompaniesByCurrentLocation";
        private const string GET_POSTERS_BY_USER_LOCATION = "getPostersByCurrentLocation";

        public UserApiService(RestClient client) : base(client)
        {

        }

        public async Task<LoginUserResponse> Login(LoginRequest data)
        {
            var response = new LoginUserResponse();

            try
            {
                var receivedData = await PostAsync(LOGIN_USER, data);

                if (!string.IsNullOrWhiteSpace(receivedData))
                {
                    var deserializedResponse = JsonConvert.DeserializeObject<LoginUserResponse>(receivedData);
                    if (deserializedResponse != null)
                    {
                        return deserializedResponse;
                    }
                }

                response.resultMsg = Result.API_SERVICE_ERROR.GetMessage();
            }
            catch (JsonException jsonEx)
            {
                response.resultMsg = $"JSON Parsing Error: {jsonEx.Message}";
            }
            catch (Exception ex)
            {
                response.resultMsg = $"Login Error: {ex.Message}";
            }

            return response;
        }
    }
}
