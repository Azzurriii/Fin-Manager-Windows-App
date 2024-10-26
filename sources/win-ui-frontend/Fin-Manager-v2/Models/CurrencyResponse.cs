using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fin_Manager_v2.Models
{
    public class CurrencyResponse
    {
        public CurrencyInfo From { get; set; }
        public CurrencyInfo To { get; set; }
        public decimal Rate { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class CurrencyInfo
    {
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public string Formatted { get; set; }
    }

}
