using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fin_Manager_v2.Converters
{
    public class ProgressColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is double percentage)
            {
                if (percentage <= 0)
                    return new SolidColorBrush(Microsoft.UI.Colors.LightGray);
                else if (percentage <= 50)
                {
                    // Chuyển từ đỏ cam sang cam khi 0-50%
                    byte red = (byte)(255 * (percentage / 50));
                    return new SolidColorBrush(Microsoft.UI.Colors.Tomato);
                }
                else if (percentage <= 100)
                {
                    // Chuyển từ cam sang xanh lá khi 50-100%
                    return new SolidColorBrush(Microsoft.UI.Colors.Green);
                }
                else
                    return new SolidColorBrush(Microsoft.UI.Colors.Purple);
            }
            return new SolidColorBrush(Microsoft.UI.Colors.LightGray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
