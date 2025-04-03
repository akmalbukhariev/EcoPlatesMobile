using Newtonsoft.Json;
using RestSharp;
using System.Text;

namespace EcoPlatesMobile.Services
{
    public class ApiService
    {
        private readonly RestClient _client;
        string token = "eyJhbGciOiJIUzI1NiJ9.eyJzdWIiOiIwMTA4NDQxMDY5NyIsImF1dGgiOiJST0xFX1VTRVIiLCJleHAiOjE3NDM5NDE5MzJ9.8mOwNu1B5IXjD4n8MARiLQPTdSYEztdA97thJVhutLQ";
        public ApiService(RestClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Stores the token securely
        /// </summary>
        public async Task SetTokenAsync(string token)
        {
            await SecureStorage.SetAsync("auth_token", token);
        }

        /// <summary>
        /// Retrieves the stored token
        /// </summary>
        public async Task<string?> GetTokenAsync()
        {
            return await SecureStorage.GetAsync("auth_token");
        }

        /// <summary>
        /// Clears the stored token (for logout)
        /// </summary>
        public Task ClearTokenAsync()
        {
            SecureStorage.Remove("auth_token");
            return Task.CompletedTask;
        }

        private async Task<RestRequest> CreateRequestAsync(string endpoint, Method method, bool includeToken = true)
        {
            var request = new RestRequest(endpoint, method);
            request.AddHeader("Accept", "application/json");

            if (includeToken)
            {
                string? token = await GetTokenAsync();
                if (!string.IsNullOrEmpty(token))
                {
                    request.AddHeader("Authorization", $"Bearer {token}");
                }
            }

            return request;
        }

        private async Task<string> ExecuteRequestAsync(RestRequest request)
        { 
            var response = await _client.ExecuteAsync(request);
            Console.WriteLine($"Status Code: {response.StatusCode}");
            Console.WriteLine($"Content Length: {response.RawBytes?.Length}");
            Console.WriteLine($"Response Headers: {string.Join(", ", response.Headers.Select(h => $"{h.Name}: {h.Value}"))}");
            Console.WriteLine("Raw Content:");
            if (response.RawBytes != null && response.RawBytes.Length > 0)
            {
                var json = Encoding.UTF8.GetString(response.RawBytes);
                Console.WriteLine("Decoded JSON:");
                Console.WriteLine(json);
                return json;
            }

            return response.Content ?? string.Empty;
        }

        public async Task<string> GetAsync(string endpoint, bool includeToken = true)
        {
            var request = await CreateRequestAsync(endpoint, Method.Get, includeToken);
            return await ExecuteRequestAsync(request);
        }

        public async Task<string> PostAsync(string endpoint, object? data = null)
        {         
            var request = new RestRequest(endpoint, Method.Post);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", $"Bearer {token}");

            if (data != null)
            {
                var json = JsonConvert.SerializeObject(data);
                request.AddJsonBody(json);
            }
            
            return await ExecuteRequestAsync(request);
        }

        public async Task<string> PutAsync(string endpoint, object? data = null)
        {
            var request = new RestRequest(endpoint, Method.Put);
            request.AddHeader("Content-Type", "application/json");

            if (data != null)
            {
                var json = JsonConvert.SerializeObject(data);
                request.AddJsonBody(json);
            }

            return await ExecuteRequestAsync(request);
        }

        public async Task<string> DeleteAsync(string endpoint)
        {
            var request =  new RestRequest(endpoint, Method.Delete);
            return await ExecuteRequestAsync(request);
        }

        /// <summary>
        /// Generic login method that allows different response types.
        /// </summary>
        public async Task<T?> LoginAsync<T>(string endpoint, object data) where T : class
        {
            try
            {
                var request = new RestRequest(endpoint, Method.Post);
                request.AddHeader("Content-Type", "application/json");
                var json = JsonConvert.SerializeObject(data);
                request.AddJsonBody(json);

                var response = await _client.ExecuteAsync(request);

                if (response.IsSuccessful && !string.IsNullOrWhiteSpace(response.Content))
                {
                    var result = JsonConvert.DeserializeObject<T>(response.Content);

                    // Extract token from headers
                    if (response.Headers != null)
                    {
                        var tokenHeader = response.Headers.FirstOrDefault(h => h.Name == "access-token");
                        if (tokenHeader != null && tokenHeader.Value != null)
                        {
                            string token = tokenHeader.Value.ToString();
                            await SetTokenAsync(token);
                        }
                    }

                    return result;
                }
            }
            catch (JsonException jsonEx)
            {
                Console.WriteLine($"JSON Parsing Error: {jsonEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login Error: {ex.Message}");
            }

            return null;
        }
    }
}
