using EcoPlatesMobile.Models.Requests;
using EcoPlatesMobile.Models.Requests.Company;
using EcoPlatesMobile.Models.Requests.User;
using EcoPlatesMobile.Models.Responses;
using EcoPlatesMobile.Models.Responses.Company;
using EcoPlatesMobile.Models.Responses.User;
using EcoPlatesMobile.Utilities;
using Newtonsoft.Json;
using RestSharp;

namespace EcoPlatesMobile.Services.Api
{
    public class UserApiService : ApiService
    {
        private const string BASE_URL = "/ecoplatesuser/api/v1/";
        private const string LOGIN_USER = $"{BASE_URL}user/login";
        private const string CHECK_USER = $"{BASE_URL}user/checkUser/";
        private const string LOGOUT_USER = $"{BASE_URL}user/logout";
        private const string REGISTER_USER = $"{BASE_URL}user/register";
        private const string GET_USER_INFO = $"{BASE_URL}user/getUserByToken";
        private const string UPDATE_USER_INFO = $"{BASE_URL}user/updateUserInfo";
        private const string REGISTER_BOOKMARK_PROMOTION = $"{BASE_URL}bookmark/registerBookmarkPromotion";
        private const string SAVE_OR_UPDATE_BOOKMARK_PROMOTION = $"{BASE_URL}bookmark/saveOrUpdateBookmarkPromotion";
        private const string SAVE_OR_UPDATE_BOOKMARK_COMPANY = $"{BASE_URL}bookmark/saveOrUpdateBookmarkCompany";
        private const string GET_USER_BOOKMARK_PROMOTION = $"{BASE_URL}bookmark/getUserBookmarkPromotion";
        private const string GET_USER_BOOKMARK_COMPANY = $"{BASE_URL}bookmark/getUserBookmarkCompany";
        private const string GET_COMPANIES_BY_USER_LOCATION = $"{BASE_URL}company/getCompaniesByCurrentLocation";
        private const string GET_POSTERS_BY_USER_LOCATION = $"{BASE_URL}promotions/getPostersByCurrentLocation";
        private const string GET_SPECIFIC_PROMOTION_WITH_COMPANY_INFO = $"{BASE_URL}promotions/getSpecificPromotionWithCompanyInfo";
        private const string GET_COMPANY_WITH_POSTERS = $"{BASE_URL}company/getCompanyWithPosters/";
        private const string REGISTER_POSTER_FEEDBACK = $"{BASE_URL}feedbacks/registerPosterFeedback";

        public UserApiService(RestClient client) : base(client)
        {
            token = "eyJhbGciOiJIUzI1NiJ9.eyJzdWIiOiIwMTA4NDQxMDY5NyIsImF1dGgiOiJST0xFX1VTRVIiLCJleHAiOjE3Nzg4MjkyOTZ9.t4cxo0f8fXsrXE5CgXi_pXBMyoaQ7tv4d7xcrnU6p0U";
        }

        public async Task<LoginUserResponse> Login(LoginRequest data)
        {
            var response = await LoginAsync<LoginUserResponse>(LOGIN_USER, data);

            return response ?? new LoginUserResponse
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
                var receivedData = await GetAsync($"{CHECK_USER}{phoneNumber}", false);

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
                var receivedData = await PostAsync(LOGOUT_USER, null);

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

        public async Task<Response> RegisterUser(RegisterUserRequest data)
        {
            var response = new Response();

            try
            {
                var receivedData = await PostAsync(REGISTER_USER, data);

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

        public async Task<GetUserInfoResponse> GetUserInfo()
        {
            var response = new GetUserInfoResponse();

            try
            {
                var receivedData = await GetAsync(GET_USER_INFO);

                if (!string.IsNullOrWhiteSpace(receivedData))
                {
                    var deserializedResponse = JsonConvert.DeserializeObject<GetUserInfoResponse>(receivedData);
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

        public async Task<Response> UpdateUserInfo(UpdateUserInfoRequest data)
        {
            var response = new Response();

            try
            {
                var receivedData = await PostAsync(UPDATE_USER_INFO, data);

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

        public async Task<Response> RegisterUserBookmarkPromotion(RegisterBookmarkPropmotionRequest data)
        {
            var response = new Response();

            try
            {
                var receivedData = await PostAsync(REGISTER_BOOKMARK_PROMOTION, data);

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

        public async Task<Response> UpdateUserBookmarkPromotionStatus(SaveOrUpdateBookmarksPromotionRequest data)
        {
            var response = new Response();

            try
            {
                var receivedData = await PostAsync(SAVE_OR_UPDATE_BOOKMARK_PROMOTION, data);

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

        public async Task<Response> UpdateUserBookmarkCompanyStatus(SaveOrUpdateBookmarksCompanyRequest data)
        {
            var response = new Response();

            try
            {
                var receivedData = await PostAsync(SAVE_OR_UPDATE_BOOKMARK_COMPANY, data);

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

        public async Task<BookmarkPromotionListResponse> GetUserBookmarkPromotion(PaginationWithLocationRequest data)
        {
            var response = new BookmarkPromotionListResponse();

            try
            {
                var receivedData = await PostAsync(GET_USER_BOOKMARK_PROMOTION, data);

                if (!string.IsNullOrWhiteSpace(receivedData))
                {
                    var deserializedResponse = JsonConvert.DeserializeObject<BookmarkPromotionListResponse>(receivedData);
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

        public async Task<BookmarkCompanyListResponse> GetUserBookmarkCompany(PaginationWithLocationRequest data)
        {
            var response = new BookmarkCompanyListResponse();

            try
            {
                var receivedData = await PostAsync(GET_USER_BOOKMARK_COMPANY, data);

                if (!string.IsNullOrWhiteSpace(receivedData))
                {
                    var deserializedResponse = JsonConvert.DeserializeObject<BookmarkCompanyListResponse>(receivedData);
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
         
        public async Task<CompanyListResponse> GetCompaniesByCurrentLocation(CompanyLocationRequest data)
        {
            var response = new CompanyListResponse();

            try
            {
                var receivedData = await PostAsync(GET_COMPANIES_BY_USER_LOCATION, data);

                if (!string.IsNullOrWhiteSpace(receivedData))
                {
                    var deserializedResponse = JsonConvert.DeserializeObject<CompanyListResponse>(receivedData);
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

        public async Task<PosterListResponse> GetPostersByCurrentLocation(PosterLocationRequest data)
        {
            var response = new PosterListResponse();

            try
            {
                var receivedData = await PostAsync(GET_POSTERS_BY_USER_LOCATION, data);

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

        public async Task<SpecificPromotionWithCompanyInfoResponse> GetSpecificPromotionWithCompanyInfo(PosterGetFeedbackRequest data)
        {
            var response = new SpecificPromotionWithCompanyInfoResponse();

            try
            {
                var receivedData = await PostAsync(GET_SPECIFIC_PROMOTION_WITH_COMPANY_INFO, data);

                if (!string.IsNullOrWhiteSpace(receivedData))
                {
                    var deserializedResponse = JsonConvert.DeserializeObject<SpecificPromotionWithCompanyInfoResponse>(receivedData);
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

        public async Task<CompanyWithPosterListResponse> getCompanyWithPosters(int company_id)
        {
            var response = new CompanyWithPosterListResponse();

            try
            {
                var receivedData = await GetAsync($"{GET_COMPANY_WITH_POSTERS}{company_id}");

                if (!string.IsNullOrWhiteSpace(receivedData))
                {
                    var deserializedResponse = JsonConvert.DeserializeObject<CompanyWithPosterListResponse>(receivedData);
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

        public async Task<Response> RegisterPosterFeedBack(RegisterPosterFeedbackRequest data)
        {
            var response = new Response();

            try
            {
                var receivedData = await PostAsync(REGISTER_POSTER_FEEDBACK, data);

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
    }
}