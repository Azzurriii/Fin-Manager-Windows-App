using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Fin_Manager_v2.Model;
using Fin_Manager_v2.Services.Interface;

namespace Fin_Manager_v2.ViewModels;

public partial class CurrencyViewModel : ObservableObject
{
    private readonly ICurrencyService _currencyService;

    [ObservableProperty]
    private decimal _amount;

    [ObservableProperty]
    private string _selectedFromCurrency;

    [ObservableProperty]
    private string _selectedToCurrency;

    [ObservableProperty]
    private CurrencyResponse _result;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage;

    public List<string> CurrencyList { get; } = new List<string>
        { "VND", "USD", "EUR", "JPY", "GBP", "AUD", "CAD", "CHF", "CNY", "SEK" };

    public CurrencyViewModel(ICurrencyService currencyService)
    {
        _currencyService = currencyService ?? throw new ArgumentNullException(nameof(currencyService));

        // Set default values
        SelectedFromCurrency = CurrencyList.FirstOrDefault() ?? "VND";
        SelectedToCurrency = CurrencyList.Skip(1).FirstOrDefault() ?? "USD";
        Amount = 0;
    }

    [RelayCommand(CanExecute = nameof(CanConvert))]
    private async Task ConvertAsync()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            if (_currencyService == null)
            {
                throw new InvalidOperationException("Currency service is not initialized");
            }

            Result = await _currencyService.ConvertCurrencyAsync(
                Amount,
                SelectedFromCurrency,
                SelectedToCurrency);
        }
        catch (HttpRequestException ex)
        {
            ErrorMessage = "Không thể kết nối đến server. Vui lòng kiểm tra lại kết nối.";
            Result = null;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Có lỗi xảy ra: {ex.Message}";
            Result = null;
            System.Diagnostics.Debug.WriteLine($"Convert error: {ex}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private bool CanConvert()
    {
        return Amount > 0 &&
               !string.IsNullOrEmpty(SelectedFromCurrency) &&
               !string.IsNullOrEmpty(SelectedToCurrency) &&
               SelectedFromCurrency != SelectedToCurrency &&
               !IsLoading;
    }

    partial void OnAmountChanged(decimal value)
    {
        ConvertCommand.NotifyCanExecuteChanged();
    }

    partial void OnSelectedFromCurrencyChanged(string value)
    {
        ConvertCommand.NotifyCanExecuteChanged();
    }

    partial void OnSelectedToCurrencyChanged(string value)
    {
        ConvertCommand.NotifyCanExecuteChanged();
    }
}
