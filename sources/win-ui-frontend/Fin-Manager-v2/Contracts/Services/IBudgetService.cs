using Fin_Manager_v2.DTO;
using Fin_Manager_v2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fin_Manager_v2.Contracts.Services
{
    public interface IBudgetService
    {
        Task<BudgetModel?> CreateBudgetAsync(CreateBudgetDto budget);
        Task<List<BudgetModel>> GetBudgetsAsync();
    }
}
