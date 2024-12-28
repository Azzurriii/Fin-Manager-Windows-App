using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Fin_Manager_v2.Models;

public class AIAnalysisModel
{
    [JsonPropertyName("ai_advice")]
    public AIAdvice AIAdvice { get; set; }
}

public class AIAdvice
{
    [JsonPropertyName("summary")]
    public string Summary { get; set; }

    [JsonPropertyName("spending_advice")]
    public string SpendingAdvice { get; set; }

    [JsonPropertyName("income_advice")]
    public string IncomeAdvice { get; set; }

    [JsonPropertyName("savings_recommendations")]
    public string SavingsRecommendations { get; set; }

    [JsonPropertyName("action_items")]
    public List<string> ActionItems { get; set; }
}
