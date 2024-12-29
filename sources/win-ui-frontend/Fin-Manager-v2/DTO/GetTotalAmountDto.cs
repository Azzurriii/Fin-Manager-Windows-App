using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Fin_Manager_v2.DTO
{
    public class GetTotalAmountDto
    {
        [JsonPropertyName("user_id")]
        public int UserId { get; set; }

        [JsonPropertyName("account_id")]
        public int? AccountId { get; set; }

        [JsonPropertyName("transaction_type")]
        public string TransactionType { get; set; } = "EXPENSE";

        [JsonPropertyName("startDate")]
        public DateTime? StartDate { get; set; }

        [JsonPropertyName("endDate")]
        public DateTime? EndDate { get; set; }

    }
}
