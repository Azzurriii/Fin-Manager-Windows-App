using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Fin_Manager_v2.DTO
{
    public class CreateFinancialGoalDto
    {
        [JsonPropertyName("goal_name")]
        [Required(ErrorMessage = "Goal name is required")]
        [StringLength(100, ErrorMessage = "Goal name cannot be longer than 100 characters")]
        [MinLength(1, ErrorMessage = "Goal name cannot be empty")]
        public string GoalName { get; set; }

        [JsonPropertyName("target_amount")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Target amount must be positive")]
        public decimal TargetAmount { get; set; }

        [JsonPropertyName("saved_amount")]
        [Range(0, double.MaxValue, ErrorMessage = "Saved amount must be positive")]
        public decimal? SavedAmount { get; set; }

        [JsonPropertyName("deadline")]
        public DateTime? Deadline { get; set; }

        // Phương thức kiểm tra validation thủ công
        //public bool IsValid()
        //{
        //    // Kiểm tra goal_name
        //    if (string.IsNullOrWhiteSpace(GoalName) || GoalName.Length > 100)
        //        return false;

        //    // Kiểm tra target_amount
        //    if (TargetAmount <= 0)
        //        return false;

        //    // Kiểm tra saved_amount (nếu có)
        //    if (SavedAmount.HasValue && SavedAmount.Value < 0)
        //        return false;

        //    return true;
        //}
    }
}
