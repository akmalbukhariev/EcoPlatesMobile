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
        private readonly RestClient _client;
        private string _token = "";

        public ApiService(RestClient client)
        {
            _client = client;
        }

        public void SetToken(string token)
        {
            _token = token;
        }

        private RestRequest CreateRequest(string endpoint, Method method)
        {
            var request = new RestRequest(endpoint, method);
            request.AddHeader("Accept", "application/json");
            if (!string.IsNullOrEmpty(_token))
            {
                request.AddHeader("Authorization", $"Bearer {_token}");
            }
            return request;
        }

        public async Task<string> GetAsync(string endpoint)
        {
            var request = CreateRequest(endpoint, Method.Get);
            request.AddHeader("Accept", "application/json");

            var response = await _client.ExecuteAsync(request);
            return response.Content ?? string.Empty;
        }

        public async Task<string> PostAsync(string endpoint, object data)
        {
            var request = CreateRequest(endpoint, Method.Post);
            request.AddHeader("Accept", "application/json");
            request.AddJsonBody(data);

            var response = await _client.ExecuteAsync(request);
            return response.Content ?? string.Empty;
        }

        public async Task<string> PutAsync(string endpoint, object data)
        {
            var request = CreateRequest(endpoint, Method.Put);
            request.AddHeader("Accept", "application/json");
            request.AddJsonBody(data);

            var response = await _client.ExecuteAsync(request);
            return response.Content ?? string.Empty;
        }

        public async Task<string> DeleteAsync(string endpoint)
        {
            var request =  CreateRequest(endpoint, Method.Delete);
            request.AddHeader("Accept", "application/json");

            var response = await _client.ExecuteAsync(request);
            return response.Content ?? string.Empty;
        }
    }

}
