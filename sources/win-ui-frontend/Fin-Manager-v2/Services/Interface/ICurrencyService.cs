using Fin_Manager_v2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fin_Manager_v2.Services.Interface
{
    public interface ICurrencyService
    {
        Task<CurrencyResponse> ConvertCurrencyAsync(decimal amount, string fromCurrency, string toCurrency);
    }
}
