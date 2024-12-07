using System.Text.Json.Serialization;

namespace Fin_Manager_v2.DTO;

public class UpdateJobDto
{

    [JsonPropertyName("job_name")]
    public string JobName { get; set; } = string.Empty;

    [JsonPropertyName("job_description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("tag_id")]
    public int TagId { get; set; }

    [JsonPropertyName("account_id")]
    public int AccountId { get; set; }

    [JsonPropertyName("user_id")]
    public int UserId { get; set; }

    [JsonPropertyName("schedule_date")]
    public string ScheduleDate { get; set; } = string.Empty;


    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("recurring_type")]
    public string RecurringType { get; set; } = string.Empty;

    [JsonPropertyName("transaction_type")]
    public string TransactionType { get; set; } = string.Empty;
} 