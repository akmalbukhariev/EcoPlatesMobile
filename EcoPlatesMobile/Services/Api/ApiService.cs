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

        /*
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
        */

        private async Task SetToken(RestRequest request)
        { 
            string? token = await GetTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                request.AddHeader("Authorization", $"Bearer {token}");
            }
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

        public async Task<string> GetAsync(string endpoint, bool useToken = true)
        { 
            var request = new RestRequest(endpoint, Method.Get);
            if (useToken)
                await SetToken(request);

            return await ExecuteRequestAsync(request);
        }

        public async Task<string> PostAsync(string endpoint, object? data = null, bool addHeader = true, bool useToken = true)
        {         
            var request = new RestRequest(endpoint, Method.Post);
            if (addHeader)
                request.AddHeader("Content-Type", "application/json");

            if (useToken)
                await SetToken(request);

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
            await SetToken(request);

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
            await SetToken(request);
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
            await SetToken(request);

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
            // Read stream into byte array to allow re-use
            using var msOriginal = new MemoryStream();
            imageStream.CopyTo(msOriginal);
            byte[] imageBytes = msOriginal.ToArray();

            // Decode image and apply orientation
            using var original = SKBitmap.Decode(imageBytes);
            if (original == null)
                throw new Exception("Could not decode image.");

            // Check EXIF orientation and correct it
            using var codec = SKCodec.Create(new SKMemoryStream(imageBytes));
            var orientation = codec.EncodedOrigin;

            using var orientedBitmap = ApplyExifOrientation(original, orientation);

            int originalWidth = orientedBitmap.Width;
            int originalHeight = orientedBitmap.Height;

            // Skip resize if already small
            if (originalWidth <= maxWidth && originalHeight <= maxHeight)
            {
                using var ms = new MemoryStream();
                orientedBitmap.Encode(ms, SKEncodedImageFormat.Jpeg, quality);
                return ms.ToArray();
            }

            float ratioX = (float)maxWidth / originalWidth;
            float ratioY = (float)maxHeight / originalHeight;
            float ratio = Math.Min(ratioX, ratioY);

            int newWidth = (int)(originalWidth * ratio);
            int newHeight = (int)(originalHeight * ratio);

            var sampling = new SKSamplingOptions(SKFilterMode.Linear);
            using var resized = orientedBitmap.Resize(new SKImageInfo(newWidth, newHeight), sampling);
            if (resized == null)
                throw new Exception("Image resize failed.");

            using var image = SKImage.FromBitmap(resized);
            using var msFinal = new MemoryStream();
            image.Encode(SKEncodedImageFormat.Jpeg, quality).SaveTo(msFinal);

            return msFinal.ToArray();
        }

        private static SKBitmap ApplyExifOrientation(SKBitmap bitmap, SKEncodedOrigin origin)
        {
            SKBitmap rotated;

            switch (origin)
            {
                case SKEncodedOrigin.BottomRight: // 180°
                    rotated = new SKBitmap(bitmap.Width, bitmap.Height);
                    using (var canvas = new SKCanvas(rotated))
                    {
                        canvas.RotateDegrees(180, bitmap.Width / 2, bitmap.Height / 2);
                        canvas.DrawBitmap(bitmap, 0, 0);
                    }
                    break;

                case SKEncodedOrigin.RightTop: // 90° CW
                    rotated = new SKBitmap(bitmap.Height, bitmap.Width);
                    using (var canvas = new SKCanvas(rotated))
                    {
                        canvas.Translate(rotated.Width, 0);
                        canvas.RotateDegrees(90);
                        canvas.DrawBitmap(bitmap, 0, 0);
                    }
                    break;

                case SKEncodedOrigin.LeftBottom: // 270° CW
                    rotated = new SKBitmap(bitmap.Height, bitmap.Width);
                    using (var canvas = new SKCanvas(rotated))
                    {
                        canvas.Translate(0, rotated.Height);
                        canvas.RotateDegrees(270);
                        canvas.DrawBitmap(bitmap, 0, 0);
                    }
                    break;

                default:
                    // No rotation needed
                    return bitmap;
            }

            return rotated;
        }

        /*public static byte[] ResizeImage(Stream imageStream, int maxWidth = 1024, int maxHeight = 1024, int quality = 80)
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
        }*/

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

                var request111 = _client.BuildUri(request);
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
