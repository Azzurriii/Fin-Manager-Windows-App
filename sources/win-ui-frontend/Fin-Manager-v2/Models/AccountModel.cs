using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Fin_Manager_v2.Models
{
    public class AccountModel
    {
        [JsonPropertyName("account_id")]
        public int AccountId { get; set; }

        [JsonPropertyName("user_id")]
        public int UserId { get; set; }

        [JsonPropertyName("account_name")]
        public string? AccountName { get; set; }

        [JsonPropertyName("account_type")]
        public string? AccountType { get; set; }

        [JsonPropertyName("initial_balance")]
        public decimal InitialBalance { get; set; }

        [JsonPropertyName("current_balance")]
        public decimal CurrentBalance { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; } = string.Empty;

        [JsonPropertyName("create_at")]
        public DateTime CreateAt { get; set; } = DateTime.Now;

        [JsonPropertyName("update_at")]
        public DateTime UpdateAt { get; set; } = DateTime.Now;
    }
}
