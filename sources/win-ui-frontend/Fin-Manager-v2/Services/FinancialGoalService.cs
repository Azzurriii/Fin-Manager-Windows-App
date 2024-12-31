using Fin_Manager_v2.Contracts.Services;
using Fin_Manager_v2.Converters;
using Fin_Manager_v2.DTO;
using Fin_Manager_v2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Fin_Manager_v2.Services
{
    public class FinancialGoalService : IFinancialGoalService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;

        public FinancialGoalService(HttpClient httpClient, IAuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
        }

        private void SetAuthorizationHeader()
        {
            var token = _authService.GetAccessToken();
            if (string.IsNullOrEmpty(token))
            {
                throw new InvalidOperationException("No valid authentication token. Please log in again.");
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<List<FinancialGoalModel>> GetAllFinancialGoalsAsync()
        {
            try
            {
                SetAuthorizationHeader();

                var response = await _httpClient.GetAsync("financial_goals");
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                using (JsonDocument doc = JsonDocument.Parse(jsonString))
                {
                    var goals = new List<FinancialGoalModel>();

                    foreach (var element in doc.RootElement.EnumerateArray())
                    {
                        var goal = new FinancialGoalModel
                        {
                            GoalId = element.GetProperty("goal_id").GetInt32(),
                            UserId = element.GetProperty("user_id").GetInt32(),
                            GoalName = element.GetProperty("goal_name").GetString(),

                            TargetAmount = decimal.TryParse(
                                element.GetProperty("target_amount").GetString(),
                                out decimal targetAmount)
                                ? targetAmount
                                : 0,

                            SavedAmount = decimal.TryParse(
                                element.GetProperty("saved_amount").GetString(),
                                out decimal savedAmount)
                                ? savedAmount
                                : 0,

                            Deadline = element.GetProperty("deadline").GetDateTime()
                        };

                        System.Diagnostics.Debug.WriteLine(
                            $"Parsed Goal: " +
                            $"ID={goal.GoalId}, " +
                            $"Name={goal.GoalName}, " +
                            $"Target={goal.TargetAmount}, " +
                            $"Saved={goal.SavedAmount}"
                        );

                        goals.Add(goal);
                    }

                    return goals;
                }

            }
            catch (Exception ex)
            {
                return new List<FinancialGoalModel>();
            }
        }

        public async Task<FinancialGoalModel> CreateFinancialGoalAsync(CreateFinancialGoalDto goal)
        {
            try
            {
                SetAuthorizationHeader();

                decimal savedAmount = await GetTotalAccountBalanceByUserIdAsync();

                var modifiedGoal = new CreateFinancialGoalDto
                {
                    GoalName = goal.GoalName,
                    TargetAmount = goal.TargetAmount,
                    SavedAmount = savedAmount,
                    Deadline = goal.Deadline
                };

                var response = await _httpClient.PostAsJsonAsync("financial_goals", modifiedGoal);
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"Create Goal Response: {jsonString}");

                var goalData = JsonDocument.Parse(jsonString).RootElement;

                var createdGoal = new FinancialGoalModel
                {
                    GoalId = goalData.GetProperty("goal_id").GetInt32(),
                    UserId = goalData.GetProperty("user_id").GetInt32(),
                    GoalName = goalData.GetProperty("goal_name").GetString(),

                    TargetAmount = goalData.GetProperty("target_amount").GetDecimal(),

                    SavedAmount = decimal.Parse(
                        goalData.GetProperty("saved_amount").GetString(),
                        System.Globalization.CultureInfo.InvariantCulture
                    ),

                    Deadline = goalData.GetProperty("deadline").GetDateTime()
                };

                System.Diagnostics.Debug.WriteLine(
                    $"Parsed Created Goal Details: " +
                    $"ID={createdGoal.GoalId}, " +
                    $"Name={createdGoal.GoalName}, " +
                    $"Target={createdGoal.TargetAmount}, " +
                    $"Saved={createdGoal.SavedAmount}"
                );

                return createdGoal;
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                return null;
            }
        }

        public async Task<decimal> GetTotalAccountBalanceByUserIdAsync()
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.GetAsync($"finance-accounts/me");
                response.EnsureSuccessStatusCode();

                var accounts = await response.Content.ReadFromJsonAsync<List<AccountModel>>();

                decimal totalBalance = accounts.Sum(a => a.CurrentBalance);

                return totalBalance;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching total balance: {ex.Message}");
                return 0;
            }
        }

        public async Task<bool> DeleteFinancialGoalAsync(int goalId)
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.DeleteAsync($"financial_goals/{goalId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                return false;
            }
        }
    }
}
