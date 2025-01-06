using Microsoft.UI.Xaml.Controls;
using Fin_Manager_v2.ViewModels;

namespace Fin_Manager_v2.Controls;

public sealed partial class MailerDialog : ContentDialog
{
    public MailerViewModel ViewModel => (MailerViewModel)DataContext;

    public MailerDialog()
    {
        InitializeComponent();
    }
} 