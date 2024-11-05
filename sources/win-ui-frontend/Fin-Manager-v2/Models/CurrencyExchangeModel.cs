using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Fin_Manager_v2.Models;
public class CurrencyExchangeModel
{
    public CurrencyModel? From { get; set; }
    public CurrencyModel? To { get; set; }
    public decimal Rate { get; set; }
    public DateTime Timestamp { get; set; }
}
