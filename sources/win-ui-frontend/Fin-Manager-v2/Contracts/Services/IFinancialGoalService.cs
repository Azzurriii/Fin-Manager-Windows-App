using Fin_Manager_v2.DTO;
using Fin_Manager_v2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fin_Manager_v2.Contracts.Services
{
    public interface IFinancialGoalService
    {
        Task<List<FinancialGoalModel>> GetAllFinancialGoalsAsync();
        Task<FinancialGoalModel> CreateFinancialGoalAsync(CreateFinancialGoalDto goal);
        Task<bool> DeleteFinancialGoalAsync(int goalId);
        Task<decimal> GetTotalAccountBalanceByUserIdAsync();
    }
}
