using System.Text.Json.Serialization;

namespace Fin_Manager_v2.Models;

public class JobModel
{
    [JsonPropertyName("job_id")]
    public int JobId { get; set; }

    [JsonPropertyName("job_name")]
    public string JobName { get; set; } = string.Empty;

    [JsonPropertyName("job_description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("tag_id")]
    public int TagId { get; set; }

    [JsonPropertyName("transaction_type")]
    public string TransactionType { get; set; } = string.Empty;

    [JsonPropertyName("account_id")]
    public int AccountId { get; set; }

    [JsonPropertyName("user_id")]
    public int UserId { get; set; }

    [JsonPropertyName("schedule_date")]
    public DateTimeOffset ScheduleDate { get; set; }

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("recurring_type")]
    public string RecurringType { get; set; } = string.Empty;

    [JsonPropertyName("next_run_date")]
    public DateTimeOffset NextRunDate { get; set; }

    [JsonPropertyName("create_at")]
    public DateTimeOffset    CreateAt { get; set; }

    [JsonPropertyName("update_at")]
    public DateTimeOffset UpdateAt { get; set; } 

    [JsonPropertyName("status")]
    public bool Status { get; set; }
}