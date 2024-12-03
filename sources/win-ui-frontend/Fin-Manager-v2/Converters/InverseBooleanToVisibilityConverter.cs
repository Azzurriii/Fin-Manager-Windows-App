using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fin_Manager_v2.Converters
{
    public class InverseBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Visibility.Collapsed : Visibility.Visible;
            }

            return Visibility.Visible; // Mặc định nếu không phải bool
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            // Chúng ta không cần hỗ trợ ConvertBack trong trường hợp này
            throw new NotImplementedException();
        }
    }
}
