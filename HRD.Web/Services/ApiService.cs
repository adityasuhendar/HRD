// HRD.Web/Services/ApiService.cs
using System.Text;
using System.Text.Json;
using HRD.Web.Models.DTOs; 

namespace HRD.Web.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiService(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;

            // Set base URL dari configuration
            var apiBaseUrl = _configuration["ApiSettings:BaseUrl"];
            if (!string.IsNullOrEmpty(apiBaseUrl))
            {
                _httpClient.BaseAddress = new Uri(apiBaseUrl);
            }
        }

        public async Task<ApiResponse<T>?> GetAsync<T>(string endpoint)
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.GetAsync(endpoint);
                return await ProcessResponse<T>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<T> { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<T>?> PostAsync<T>(string endpoint, object data)
        {
            try
            {
                // DEBUG: Log what we're about to send
                Console.WriteLine($"[ApiService] ===== POST REQUEST DEBUG =====");
                Console.WriteLine($"[ApiService] Endpoint: {endpoint}");
                Console.WriteLine($"[ApiService] BaseAddress: {_httpClient.BaseAddress}");

                SetAuthorizationHeader();

                // DEBUG: Check if auth header is set
                var authHeader = _httpClient.DefaultRequestHeaders.Authorization?.ToString();
                Console.WriteLine($"[ApiService] Auth Header: {(string.IsNullOrEmpty(authHeader) ? "MISSING" : "Present")}");

                // Serialize and log the JSON payload
                var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                Console.WriteLine($"[ApiService] JSON Payload:\n{json}");

                // Check if GajiPokok is in the JSON (specific debug)
                if (json.Contains("gajiPokok") || json.Contains("GajiPokok"))
                {
                    Console.WriteLine($"[ApiService] ✅ GajiPokok found in JSON");
                }
                else
                {
                    Console.WriteLine($"[ApiService] ❌ GajiPokok NOT found in JSON");
                }

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // DEBUG: Log request details
                Console.WriteLine($"[ApiService] Content-Type: {content.Headers.ContentType}");
                Console.WriteLine($"[ApiService] Content Length: {json.Length} chars");

                // Make the request
                Console.WriteLine($"[ApiService] Sending request to: {_httpClient.BaseAddress}{endpoint}");
                var response = await _httpClient.PostAsync(endpoint, content);

                // DEBUG: Log response details
                Console.WriteLine($"[ApiService] Response Status: {response.StatusCode}");
                Console.WriteLine($"[ApiService] Response Reason: {response.ReasonPhrase}");

                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[ApiService] Response Content Length: {responseContent.Length} chars");
                Console.WriteLine($"[ApiService] Response Content:\n{responseContent}");

                // Check if response contains GajiPokok
                if (responseContent.Contains("gajiPokok") || responseContent.Contains("GajiPokok"))
                {
                    Console.WriteLine($"[ApiService] ✅ GajiPokok found in response");
                }
                else
                {
                    Console.WriteLine($"[ApiService] ❌ GajiPokok NOT found in response");
                }

                Console.WriteLine($"[ApiService] ===== END POST REQUEST DEBUG =====");

                return await ProcessResponse<T>(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ApiService] ❌ EXCEPTION: {ex.Message}");
                Console.WriteLine($"[ApiService] Stack Trace: {ex.StackTrace}");
                return new ApiResponse<T> { Success = false, Message = $"Error: {ex.Message}" };
            }
        }


        public async Task<ApiResponse<T>?> PutAsync<T>(string endpoint, object data)
        {
            try
            {
                SetAuthorizationHeader();
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync(endpoint, content);
                return await ProcessResponse<T>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<T> { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<string>?> DeleteAsync(string endpoint)
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.DeleteAsync(endpoint);
                return await ProcessResponse<string>(response);
            }
            catch (Exception ex)
            {
                return new ApiResponse<string> { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        private void SetAuthorizationHeader()
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("AuthToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        private async Task<ApiResponse<T>?> ProcessResponse<T>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<ApiResponse<T>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            else
            {
                // Try to parse error response
                try
                {
                    var errorResponse = JsonSerializer.Deserialize<ApiResponse<T>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return errorResponse;
                }
                catch
                {
                    return new ApiResponse<T>
                    {
                        Success = false,
                        Message = $"HTTP {response.StatusCode}: {response.ReasonPhrase}"
                    };
                }
            }
        }
    }
}