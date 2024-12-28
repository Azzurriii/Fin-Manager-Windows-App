using Fin_Manager_v2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Fin_Manager_v2.Contracts.Services;

public interface IAnalysisService
{
    Task<AnalysisModel> GetAnalysisAsync(int userId, int? accountId, DateTime startDate, DateTime endDate);
    Task<AIAnalysisModel> GetAIAnalysisAsync(AnalysisModel data);
}