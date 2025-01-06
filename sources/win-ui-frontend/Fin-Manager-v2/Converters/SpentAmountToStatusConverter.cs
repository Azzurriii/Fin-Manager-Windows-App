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
            if (value is BudgetModel budget)
            {
                var budgetAmount = budget.BudgetAmount ?? 0;
                var spentAmount = budget.SpentAmount ?? 0;

                if (budgetAmount == 0)
                {
                    return "No budget available.";
                }

                var difference = budgetAmount - spentAmount;

                if (difference > 0)
                {
                    return $"You have {difference} remaining before reaching the budget limit.";
                }
                else if (difference == 0)
                {
                    return "You have used the exact budget amount.";
                }
                else
                {
                    return $"You have exceeded the budget by {Math.Abs(difference):C}.";
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