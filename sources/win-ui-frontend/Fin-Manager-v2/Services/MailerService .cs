using Fin_Manager_v2.Contracts.Services;
using Fin_Manager_v2.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Fin_Manager_v2.Services
{
    public class MailerService : IMailerService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;

        public MailerService(HttpClient httpClient, IAuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
        }

        public async Task<bool> CreateMailerAsync(CreateMailerDto mailerDto)
        {
            try
            {
                // Đảm bảo token được thêm vào header
                var token = _authService.GetAccessToken();
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.PostAsJsonAsync("http://localhost:3000/mailer", mailerDto);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Error: {response.StatusCode} - {errorContent}");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                // Logging hoặc xử lý lỗi
                System.Diagnostics.Debug.WriteLine($"CreateMailer Error: {ex.Message}");
                throw;
            }
        }
    }
}
