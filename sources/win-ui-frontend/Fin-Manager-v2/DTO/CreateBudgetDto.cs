using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Fin_Manager_v2.DTO
{
    public class CreateBudgetDto
    {
        [JsonPropertyName("user_id")]
        public int? UserId { get; set; }

        [JsonPropertyName("account_id")]
        public int? AccountId { get; set; }

        [JsonPropertyName("category")]
        public string? Category { get; set; }

        [JsonPropertyName("budget_amount")]
        public decimal? BudgetAmount { get; set; }

        [JsonPropertyName("spent_amount")]
        public decimal? SpentAmount { get; set; }

        [JsonPropertyName("start_date")]
        public DateTime? StartDate { get; set; }

        [JsonPropertyName("end_date")]
        public DateTime? EndDate { get; set; }

        [JsonPropertyName("created_by")]
        public int? CreatedBy { get; set; }
    }
}
