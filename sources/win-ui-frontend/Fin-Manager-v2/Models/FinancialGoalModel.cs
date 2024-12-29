using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Fin_Manager_v2.Converters;
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveChartsCore;

namespace Fin_Manager_v2.Models
{
    public class FinancialGoalModel
    {
        [JsonProperty("goal_id")]
        public int GoalId { get; set; }

        [JsonProperty("user_id")]
        public int UserId { get; set; }

        [JsonProperty("goal_name")]
        [Required]
        [MaxLength(100)]
        public string GoalName { get; set; }

        [JsonProperty("target_amount")]
        //[JsonConverter(typeof(CustomDecimalConverter))]
        [Required]
        public decimal TargetAmount { get; set; }

        [JsonProperty("saved_amount")]
        //[JsonConverter(typeof(CustomDecimalConverter))]
        public decimal SavedAmount { get; set; } = 0;

        [JsonProperty("deadline")]
        public DateTime? Deadline { get; set; }

        [JsonProperty("create_at")]
        public DateTime CreateAt { get; set; } = DateTime.Now;

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public double CompletionPercentage
        {
            get
            {
                return TargetAmount > 0
                    ? Math.Round(decimal.ToDouble(SavedAmount / TargetAmount) * 100.0, 2)
                    : 0.0;
            }
        }

        public IEnumerable<ISeries> Series()
        {
            return GaugeGenerator.BuildSolidGauge(
                new GaugeItem(
                    CompletionPercentage,
                    series =>
                    {
                        series.MaxRadialColumnWidth = 50;
                        series.DataLabelsSize = 50;
                    }));
        }
    }
}
