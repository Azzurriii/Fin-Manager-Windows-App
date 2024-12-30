using Fin_Manager_v2.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fin_Manager_v2.Contracts.Services
{
    public interface IMailerService
    {
        Task<bool> CreateMailerAsync(CreateMailerDto mailerDto);
    }
}
