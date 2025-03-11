using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoPlatesMobile.Services
{
    public class ApiService
    {
        private readonly string _baseUrl;
        private readonly string _token;  // Pass token through constructor if needed

        public ApiService(string baseUrl, string token)
        {
            _baseUrl = baseUrl;
            _token = token;
        }

        private RestClient CreateClient()
        {
            var options = new RestClientOptions(_baseUrl);
            var client = new RestClient(options);
            return client;
        }

        public async Task<T?> GetAsync<T>(string endpoint) where T : class
        {
            var client = CreateClient();
            var request = new RestRequest(endpoint, Method.Get);
            request.AddHeader("Accept", "application/json");
            if (!string.IsNullOrEmpty(_token))
                request.AddHeader("Authorization", $"Bearer {_token}");

            var response = await client.ExecuteAsync<T>(request);
            if (response.IsSuccessful && response.Data != null)
                return response.Data;

            return null;  // Fix: Return null if response is unsuccessful
        }

        public async Task<T?> PostAsync<T>(string endpoint, object data) where T : class
        {
            var client = CreateClient();
            var request = new RestRequest(endpoint, Method.Post);
            request.AddHeader("Accept", "application/json");
            if (!string.IsNullOrEmpty(_token))
                request.AddHeader("Authorization", $"Bearer {_token}");

            request.AddJsonBody(data);

            var response = await client.ExecuteAsync<T>(request);
            if (response.IsSuccessful && response.Data != null)
                return response.Data;

            return null;  // Fix: Return null if response is unsuccessful
        }

        public async Task<T?> PutAsync<T>(string endpoint, object data) where T : class
        {
            var client = CreateClient();
            var request = new RestRequest(endpoint, Method.Put);
            request.AddHeader("Accept", "application/json");
            if (!string.IsNullOrEmpty(_token))
                request.AddHeader("Authorization", $"Bearer {_token}");

            request.AddJsonBody(data);

            var response = await client.ExecuteAsync<T>(request);
            if (response.IsSuccessful && response.Data != null)
                return response.Data;

            return null;  // Fix: Return null if response is unsuccessful
        }

        public async Task<bool> DeleteAsync(string endpoint)
        {
            var client = CreateClient();
            var request = new RestRequest(endpoint, Method.Delete);
            request.AddHeader("Accept", "application/json");
            if (!string.IsNullOrEmpty(_token))
                request.AddHeader("Authorization", $"Bearer {_token}");

            var response = await client.ExecuteAsync(request);
            return response.IsSuccessful;
        }
    }
}
