﻿using Fin_Manager_v2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fin_Manager_v2.Contracts.Services;
public interface ICurrencyService
{
    Task<CurrencyExchangeModel> ConvertCurrencyAsync(decimal amount, string fromCurrency, string toCurrency);
}

