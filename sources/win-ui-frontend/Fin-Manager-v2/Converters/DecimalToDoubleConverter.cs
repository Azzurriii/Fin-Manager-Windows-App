﻿using Microsoft.UI.Xaml.Data;
using System;

namespace Fin_Manager_v2.Converters
{
    public class DecimalToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is decimal decimalValue)
            {
                return (double)decimalValue;
            }
            return 0.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is double doubleValue)
            {
                return (decimal)doubleValue;
            }
            return 0.0m;
        }
    }
}
