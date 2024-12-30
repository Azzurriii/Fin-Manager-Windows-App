using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Fin_Manager_v2.DTO
{
    public class CreateMailerDto
    {
        [JsonPropertyName("userId")]
        [Required]
        public int? UserId { get; set; }

        [JsonPropertyName("userEmail")]
        [Required]
        [EmailAddress]
        public string? UserEmail { get; set; }

        [JsonPropertyName("title")]
        [Required]
        public string? Title { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("amount")]
        [Range(0, double.MaxValue)]
        public double Amount { get; set; }

        [JsonPropertyName("period")]
        [Required]
        public string? Period { get; set; }

        [JsonPropertyName("startDate")]
        [Required]
        public DateTime StartDate { get; set; }

        [JsonPropertyName("paymentLink")]
        [Url]
        public string? PaymentLink { get; set; }
    }

    // Enum cho Period
    public enum MailerPeriod
    {
        Monthly,
        Quarterly,
        Yearly
    }
}
