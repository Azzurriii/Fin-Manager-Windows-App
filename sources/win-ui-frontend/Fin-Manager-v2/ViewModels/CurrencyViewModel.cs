using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Fin_Manager_v2.Contracts.Services;
using Fin_Manager_v2.Models;

namespace Fin_Manager_v2.ViewModels;

public partial class CurrencyViewModel : ObservableObject
{
    private readonly ICurrencyService _currencyService;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private decimal _amount;

    [ObservableProperty]
    private string _selectedFromCurrency;

    [ObservableProperty]
    private string _selectedToCurrency;

    [ObservableProperty]
    private CurrencyModel _result;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage;

    public List<string> CurrencyList { get; } = new List<string>
        { "VND", "USD", "EUR", "JPY", "GBP", "AUD", "CAD", "CHF", "CNY", "SEK" };

    public CurrencyViewModel(ICurrencyService currencyService, IDialogService dialogService)
    {
        _currencyService = currencyService ?? throw new ArgumentNullException(nameof(currencyService));
        _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

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
            await _dialogService.ShowErrorAsync(
                "Connection Error",
                "Unable to connect to the server. Please check your connection.");
            Result = null;
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync(
                "Conversion Error",
                $"An error occurred: {ex.Message}");
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
