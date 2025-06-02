using Microsoft.Maui.Graphics.Platform;
using Newtonsoft.Json;
using RestSharp;
using SkiaSharp;
using System.Text;

namespace EcoPlatesMobile.Services
{
    public class ApiService
    {
        private readonly RestClient _client;
        protected string token = string.Empty;
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
            if (response.RawBytes != null && response.RawBytes.Length > 0)
            {
                var json = Encoding.UTF8.GetString(response.RawBytes);
                return json;
            }

            return response.Content ?? string.Empty;
        }

        public async Task<string> GetAsync(string endpoint, bool includeToken = true)
        {
            var request = await CreateRequestAsync(endpoint, Method.Get, includeToken);
            request.AddHeader("Authorization", $"Bearer {token}");

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

        public async Task<string> PostImageAsync(string endpoint, Stream imageStream, Dictionary<string, string>? additionalData = null, string streamName = "image_data")
        {
            var request = new RestRequest(endpoint, Method.Post);
            request.AddHeader("Authorization", $"Bearer {token}");
            request.AlwaysMultipartFormData = true;

            if (additionalData != null)
            {
                foreach (var entry in additionalData)
                {
                    request.AddParameter(entry.Key, entry.Value);
                }
            }

            if (imageStream != null)
            {
                //var fileBytes = await ConvertStreamToByteArrayAsync(imageStream);
                var fileBytes = ResizeImage(imageStream);
                request.AddFile(streamName, fileBytes, "image.jpg", "image/jpeg");
            }

            return await ExecuteRequestAsync(request);
        }

        public async Task<string> DeleteAsync(string endpoint)
        {
            var request =  new RestRequest(endpoint, Method.Delete);
            request.AddHeader("Authorization", $"Bearer {token}");

            return await ExecuteRequestAsync(request);
        }
         
        private async Task<byte[]> ConvertStreamToByteArrayAsync(Stream stream)
        {
            using (var memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public static byte[] ResizeImage(Stream imageStream, int maxWidth = 1024, int maxHeight = 1024, int quality = 80)
        {
            using var original = SKBitmap.Decode(imageStream);
            if (original == null)
                throw new Exception("Could not decode image.");

            int originalWidth = original.Width;
            int originalHeight = original.Height;

            // Skip resize if image is small enough
            if (originalWidth <= maxWidth && originalHeight <= maxHeight)
            {
                using var ms = new MemoryStream();
                original.Encode(ms, SKEncodedImageFormat.Jpeg, quality);
                return ms.ToArray();
            }

            float ratioX = (float)maxWidth / originalWidth;
            float ratioY = (float)maxHeight / originalHeight;
            float ratio = Math.Min(ratioX, ratioY);

            int newWidth = (int)(originalWidth * ratio);
            int newHeight = (int)(originalHeight * ratio);

            var sampling = new SKSamplingOptions(SKFilterMode.Linear); // Linear is better for scaling
            using var resized = original.Resize(new SKImageInfo(newWidth, newHeight), sampling);
            if (resized == null)
                throw new Exception("Image resize failed.");

            using var image = SKImage.FromBitmap(resized);
            using var msFinal = new MemoryStream();
            image.Encode(SKEncodedImageFormat.Jpeg, quality).SaveTo(msFinal);

            return msFinal.ToArray();
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
