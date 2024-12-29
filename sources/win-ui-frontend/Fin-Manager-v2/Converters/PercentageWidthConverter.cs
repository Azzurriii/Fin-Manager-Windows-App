using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fin_Manager_v2.Converters
{
    public class PercentageWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is double actualWidth)
            {
                double percentage = 50;
                if (parameter is double bindingPercentage)
                {
                    percentage = bindingPercentage;
                }

                return actualWidth * (percentage / 100.0);
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
