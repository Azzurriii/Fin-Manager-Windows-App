using Fin_Manager_v2.Models;
using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace Fin_Manager_v2.Converters
{
    public class SpentAmountToStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // Trường hợp value là BudgetModel
            if (value is BudgetModel budget)
            {
                var budgetAmount = budget.BudgetAmount ?? 0;
                var spentAmount = budget.SpentAmount ?? 0;

                if (budgetAmount == 0)
                {
                    return "Chưa có ngân sách.";
                }

                var difference = budgetAmount - spentAmount;

                if (difference > 0)
                {
                    return $"Bạn còn {difference:C} trước khi chạm mức ngân sách.";
                }
                else if (difference == 0)
                {
                    return "Bạn đã sử dụng đúng mức ngân sách.";
                }
                else
                {
                    return $"Bạn đã vượt ngân sách {Math.Abs(difference):C}.";
                }
            }

            return "Chưa có ngân sách.";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class SpentAmountToForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is BudgetModel budget)
            {
                decimal spentAmount = budget.SpentAmount ?? 0;
                decimal budgetAmount = budget.BudgetAmount ?? 0;

                return spentAmount <= budgetAmount
                    ? new SolidColorBrush(Colors.Green)
                    : new SolidColorBrush(Colors.Red);
            }

            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}