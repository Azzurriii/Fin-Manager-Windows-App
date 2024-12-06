using Fin_Manager_v2.Models;

namespace Fin_Manager_v2.Contracts.Services;

public interface IReportService
{
    Task<SummaryModel> GetSummaryAsync(int userId, int accountId, DateTime startDate, DateTime endDate);
    Task<List<OverviewModel>> GetOverviewAsync(int userId, int accountId, DateTime startDate, DateTime endDate);
    Task<List<CategoryReportModel>> GetCategoryReportAsync(int userId, int accountId, string type, DateTime startDate, DateTime endDate);
    (DateTime StartDate, DateTime EndDate) GetDateRangeFromPeriod(string period, DateTimeOffset selectedDate, string selectedMonth, string selectedQuarter, int selectedYear);
}