using Fin_Manager_v2.Models;

namespace Fin_Manager_v2.Contracts.Services;

public interface IDialogService
{
    Task ShowErrorAsync(string title, string message);
    Task ShowWarningAsync(string title, string message);
    Task ShowSuccessAsync(string title, string message);
    Task<bool> ShowConfirmAsync(string title, string message);
}