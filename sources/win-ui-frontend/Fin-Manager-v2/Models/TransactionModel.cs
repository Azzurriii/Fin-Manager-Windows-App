using System.Text.Json.Serialization;
using System;

namespace Fin_Manager_v2.Models;

public class TransactionModel
{
    [JsonPropertyName("transaction_id")]
    public int TransactionId { get; set; }

    [JsonPropertyName("account_id")]
    public int AccountId { get; set; }

    [JsonPropertyName("user_id")]
    public int UserId { get; set; }

    [JsonPropertyName("transaction_type")]
    public string TransactionType { get; set; }

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("tag_id")]
    public int TagId { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("transaction_date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [JsonPropertyName("account")]
    public string Account { get; set; }

    public string FormattedAmount => $"{(TransactionType == "EXPENSE" ? "-" : "+")}{Amount:C}";
}