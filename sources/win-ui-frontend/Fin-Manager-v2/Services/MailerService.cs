using System.Net.Http.Headers;
using System.Net.Http.Json;
using Fin_Manager_v2.Contracts.Services;
using Fin_Manager_v2.DTO;

namespace Fin_Manager_v2.Services;

public class MailerService : IMailerService
{
    private readonly HttpClient _httpClient;

    public MailerService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> CreateMailerAsync(MailerDto mailerDto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/mailer", mailerDto);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in CreateMailerAsync: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> DeleteMailerAsync(int id)
    {
        try 
        {
            var response = await _httpClient.DeleteAsync($"mailer/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in DeleteMailerAsync: {ex.Message}");
            throw;
        }
    }
}