namespace Fin_Manager_v2.Models;

public class CurrencyModel
{
    public string Currency { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Formatted { get; set; } = string.Empty;
}