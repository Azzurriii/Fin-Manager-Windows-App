using Fin_Manager_v2.DTO;

namespace Fin_Manager_v2.Contracts.Services;

public interface IMailerService
{
    Task<bool> CreateMailerAsync(MailerDto mailerDto);
    Task<bool> DeleteMailerAsync(int id);
}