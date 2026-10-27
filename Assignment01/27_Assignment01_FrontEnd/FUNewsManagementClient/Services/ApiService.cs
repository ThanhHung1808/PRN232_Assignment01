using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using FUNewsManagementClient.DTOs;
using FUNewsManagementClient.Helpers;

namespace FUNewsManagementClient.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClientFactory.CreateClient();
            var baseUrl = configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5000";
            _httpClient.BaseAddress = new Uri(baseUrl);
            _httpContextAccessor = httpContextAccessor;

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        private void AttachAuthToken()
        {
            var user = _httpContextAccessor.HttpContext?.Session.GetObject<LoginResponse>("CurrentUser");
            if (user != null && !string.IsNullOrWhiteSpace(user.Token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        public async Task<ApiResponse<T>> GetAsync<T>(string endpoint)
        {
            try
            {
                AttachAuthToken();
                var response = await _httpClient.GetAsync(endpoint);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var data = JsonSerializer.Deserialize<T>(content, _jsonOptions);
                    return new ApiResponse<T>
                    {
                        IsSuccess = true,
                        StatusCode = (int)response.StatusCode,
                        Data = data
                    };
                }

                return HandleError<T>(response, content);
            }
            catch (Exception ex)
            {
                return new ApiResponse<T>
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    ErrorMessage = $"Connection error: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<T>> PostAsync<T>(string endpoint, object data)
        {
            try
            {
                AttachAuthToken();
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(endpoint, content);
                var resContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var resultData = JsonSerializer.Deserialize<T>(resContent, _jsonOptions);
                    return new ApiResponse<T>
                    {
                        IsSuccess = true,
                        StatusCode = (int)response.StatusCode,
                        Data = resultData
                    };
                }

                return HandleError<T>(response, resContent);
            }
            catch (Exception ex)
            {
                return new ApiResponse<T>
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    ErrorMessage = $"Connection error: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<T>> PutAsync<T>(string endpoint, object data)
        {
            try
            {
                AttachAuthToken();
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync(endpoint, content);
                var resContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var resultData = JsonSerializer.Deserialize<T>(resContent, _jsonOptions);
                    return new ApiResponse<T>
                    {
                        IsSuccess = true,
                        StatusCode = (int)response.StatusCode,
                        Data = resultData
                    };
                }

                return HandleError<T>(response, resContent);
            }
            catch (Exception ex)
            {
                return new ApiResponse<T>
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    ErrorMessage = $"Connection error: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(string endpoint)
        {
            try
            {
                AttachAuthToken();
                var response = await _httpClient.DeleteAsync(endpoint);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return new ApiResponse<bool>
                    {
                        IsSuccess = true,
                        StatusCode = (int)response.StatusCode,
                        Data = true
                    };
                }

                var err = HandleError<bool>(response, content);
                err.Data = false;
                return err;
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Data = false,
                    ErrorMessage = $"Connection error: {ex.Message}"
                };
            }
        }

        private ApiResponse<T> HandleError<T>(HttpResponseMessage response, string content)
        {
            string message = "An error occurred.";
            try
            {
                using var doc = JsonDocument.Parse(content);
                if (doc.RootElement.TryGetProperty("message", out var msgProp))
                {
                    message = msgProp.GetString() ?? message;
                }
                else if (doc.RootElement.TryGetProperty("title", out var titleProp))
                {
                    message = titleProp.GetString() ?? message;
                }
                else
                {
                    message = content;
                }
            }
            catch
            {
                if (!string.IsNullOrWhiteSpace(content)) message = content;
            }

            return new ApiResponse<T>
            {
                IsSuccess = false,
                StatusCode = (int)response.StatusCode,
                ErrorMessage = message
            };
        }
    }
}
