using Microsoft.UI.Xaml.Data;

namespace Fin_Manager_v2.Converters;

public class TypeToIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return (value as string) switch
        {
            "INCOME" => "\uE10B", // Upward arrow
            "EXPENSE" => "\uE10C", // Downward arrow
            _ => "\uE10F"  // Default circle
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}