using EcoPlatesMobile.Models.Company;
using EcoPlatesMobile.Models.Requests;
using EcoPlatesMobile.Models.Requests.Company;
using EcoPlatesMobile.Models.Responses;
using EcoPlatesMobile.Models.Responses.Company;
using EcoPlatesMobile.Utilities;
using Newtonsoft.Json;
using RestSharp;

namespace EcoPlatesMobile.Services.Api
{
    public class CompanyApiService : ApiService
    {
        #region Url
        private const string BASE_URL = "";
        //private const string BASE_URL = "ecoplatescompany/api/v1/";
        private const string LOGIN_COMPANY = $"{BASE_URL}company/login";
        private const string CHECK_COMPANY = $"{BASE_URL}company/checkUser/";
        private const string LOGOUT_COMPANY = $"{BASE_URL}company/logout";
        private const string REGISTER_COMPANY = $"{BASE_URL}company/registerCompany";
        private const string GET_COMPANY = $"{BASE_URL}company/getComapnyInfo";
        private const string UPDATE_COMPANY_INFO = $"{BASE_URL}company/updateUserInfo";
        private const string UPDATE_COMPANY_PHONE_NUMBER = $"{BASE_URL}company/updateCompanyPhoneNumber/";
        private const string REGISTER_POSTER = $"{BASE_URL}poster/registerPoster";
        private const string UPDATE_POSTER = $"{BASE_URL}poster/updatePoster";
        private const string GET_POSTER = $"{BASE_URL}poster/getCompanyPoster";
        private const string DELETE_POSTER = $"{BASE_URL}poster/deletePoster/";
        private const string CHANGE_POSTER_DELETION_STATUS = $"{BASE_URL}poster/changePosterDeletionStatus";
        private const string GET_COMPANY_PROFILE_INFO = $"{BASE_URL}company/getCompanyProfileInfo/";
        private const string UPDATE_COMPANY_PROFILE_INFO = $"{BASE_URL}company/updateCompanyInfo";
        private const string REGISTER_COMPANY_FEEDBACK = $"{BASE_URL}feedbacks_company/registerCompanyFeedback";

        private const string GET_SENDER_ID_LIST = $"{Constants.BASE_CHAT_URL}/ecoplateschatting/api/v1/chat/getSenderIdList/";
        #endregion

        public CompanyApiService(RestClient client) : base(client)
        {
            //token = "eyJhbGciOiJIUzI1NiJ9.eyJzdWIiOiI5OTgxMjM0NTY3ODk4IiwiYXV0aCI6IlJPTEVfQ09NUEFOWSIsImV4cCI6MTc4MDQ4MzEzMH0.6PFhl1xAwm5wnlrtwDMYA4o2j0vOC9fdC5_uS3EWzCk";
        }

        public async Task<LoginCompanyResponse> Login(LoginRequest data)
        {
            var response = await LoginAsync<LoginCompanyResponse>(LOGIN_COMPANY, data);

            return response ?? new LoginCompanyResponse
            {
                resultCode = ApiResult.LOGIN_FAILED.GetCodeToString(),
                resultMsg = ApiResult.LOGIN_FAILED.GetMessage()
            };
        }

        public async Task<Response> CheckUser(string phoneNumber)
        {
            var response = new Response();

            try
            {
                var receivedData = await GetAsync($"{CHECK_COMPANY}{phoneNumber}");

                if (!string.IsNullOrWhiteSpace(receivedData))
                {
                    var deserializedResponse = JsonConvert.DeserializeObject<Response>(receivedData);
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

        public async Task<Response> LogOut()
        {
            var response = new Response();

            try
            {
                var receivedData = await PostAsync(LOGOUT_COMPANY, null, false);

                if (!string.IsNullOrWhiteSpace(receivedData))
                {
                    var deserializedResponse = JsonConvert.DeserializeObject<Response>(receivedData);
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

        public async Task<Response> UpdateCompanyPhoneNumber(string new_phone_number)
        {
            var response = new Response();

            try
            {
                var receivedData = await PostAsync($"{UPDATE_COMPANY_PHONE_NUMBER}{new_phone_number}", null, false);

                if (!string.IsNullOrWhiteSpace(receivedData))
                {
                    var deserializedResponse = JsonConvert.DeserializeObject<Response>(receivedData);
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

        public async Task<Response> RegisterCompany(Stream imageStream, Dictionary<string, string>? additionalData)
        {
            var response = new Response();

            try
            {
                var receivedData = await PostImageAsync(REGISTER_COMPANY, imageStream, additionalData, "logo_data");

                if (!string.IsNullOrWhiteSpace(receivedData))
                {
                    var deserializedResponse = JsonConvert.DeserializeObject<Response>(receivedData);
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
                response.resultMsg = $"RegisterPoster Error: {ex.Message}";
            }

            return response;
        }

        public async Task<GetCompanyInfoResponse> GetCompanyInfo()
        {
            var response = new GetCompanyInfoResponse();

            try
            {
                var receivedData = await GetAsync(GET_COMPANY);

                if (!string.IsNullOrWhiteSpace(receivedData))
                {
                    var deserializedResponse = JsonConvert.DeserializeObject<GetCompanyInfoResponse>(receivedData);
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

        public async Task<Response> UpdateUserInfo(UpdateCompanyInfoRequest data)
        {
            var response = new Response();

            try
            {
                var receivedData = await PostAsync(UPDATE_COMPANY_INFO, data);

                if (!string.IsNullOrWhiteSpace(receivedData))
                {
                    var deserializedResponse = JsonConvert.DeserializeObject<Response>(receivedData);
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

        public async Task<Response> RegisterPoster(Stream imageStream, Dictionary<string, string>? additionalData)
        {
            var response = new Response();

            try
            {
                var receivedData = await PostImageAsync(REGISTER_POSTER, imageStream, additionalData);

                if (!string.IsNullOrWhiteSpace(receivedData))
                {
                    var deserializedResponse = JsonConvert.DeserializeObject<Response>(receivedData);
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
                response.resultMsg = $"RegisterPoster Error: {ex.Message}";
            }

            return response;
        }

        public async Task<Response> UpdatePoster(Stream imageStream, Dictionary<string, string>? additionalData)
        {
            var response = new Response();

            try
            {
                var receivedData = await PostImageAsync(UPDATE_POSTER, imageStream, additionalData);

                if (!string.IsNullOrWhiteSpace(receivedData))
                {
                    var deserializedResponse = JsonConvert.DeserializeObject<Response>(receivedData);
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
                response.resultMsg = $"UpdatePoster Error: {ex.Message}";
            }

            return response;
        }

        public async Task<PosterListResponse> GetCompanyPoster(PaginationWithDeletedParam data)
        {
            var response = new PosterListResponse();

            try
            {
                var receivedData = await PostAsync(GET_POSTER, data);

                if (!string.IsNullOrWhiteSpace(receivedData))
                {
                    var deserializedResponse = JsonConvert.DeserializeObject<PosterListResponse>(receivedData);
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

        /*
        public async Task<CompanyProfileInfoResponse> GetCompanyProfileInfo(int company_id)
        {
            var response = new CompanyProfileInfoResponse();

            try
            {
                var receivedData = await GetAsync($"{GET_COMPANY_PROFILE_INFO}{company_id}");

                if (!string.IsNullOrWhiteSpace(receivedData))
                {
                    var deserializedResponse = JsonConvert.DeserializeObject<CompanyProfileInfoResponse>(receivedData);
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
                response.resultMsg = $"CompanyProfileInfoResponse Error: {ex.Message}";
            }

            return response;
        }
        */

        public async Task<Response> RegisterCompanyFeedBack(CompanyFeedbackInfoRequest data)
        {
            var response = new Response();

            try
            {
                var receivedData = await PostAsync(REGISTER_COMPANY_FEEDBACK, data);

                if (!string.IsNullOrWhiteSpace(receivedData))
                {
                    var deserializedResponse = JsonConvert.DeserializeObject<Response>(receivedData);
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

        public async Task<Response> UpdateCompanyProfileInfo(Stream imageStream, Dictionary<string, string>? additionalData)
        {
            var response = new Response();

            try
            {
                var receivedData = await PostImageAsync(UPDATE_COMPANY_PROFILE_INFO, imageStream, additionalData, "logo_data");

                if (!string.IsNullOrWhiteSpace(receivedData))
                {
                    var deserializedResponse = JsonConvert.DeserializeObject<Response>(receivedData);
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
                response.resultMsg = $"UpdateCompanyProfileInfo Error: {ex.Message}";
            }

            return response;
        }

        public async Task<Response> ChangePosterDeletionStatus(ChangePosterDeletionRequest data)
        {
            var response = new Response();

            try
            {
                var receivedData = await PostAsync(CHANGE_POSTER_DELETION_STATUS, data);

                if (!string.IsNullOrWhiteSpace(receivedData))
                {
                    var deserializedResponse = JsonConvert.DeserializeObject<Response>(receivedData);
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
                response.resultMsg = $"ChangePosterDeletionStatus Error: {ex.Message}";
            }

            return response;
        }

        public async Task<Response> DeletePoster(long poster_id)
        {
            var response = new Response();

            try
            {
                var receivedData = await DeleteAsync($"{DELETE_POSTER}{poster_id}");

                if (!string.IsNullOrWhiteSpace(receivedData))
                {
                    var deserializedResponse = JsonConvert.DeserializeObject<Response>(receivedData);
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
   
   
        /////////////////////Chat////////////////////
        public async Task<ChatMessageResponse> GetSenderIdList(long receiver_id)
        {
            var response = new ChatMessageResponse();

            try
            {
                var receivedData = await GetAsync($"{GET_SENDER_ID_LIST}{receiver_id}");

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
                response.resultMsg = $"Login Error: {ex.Message}";
            }

            return response;
        }
   
   
    }
}