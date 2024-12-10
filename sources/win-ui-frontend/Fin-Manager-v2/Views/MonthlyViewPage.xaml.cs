using Fin_Manager_v2.ViewModels;
using Fin_Manager_v2.Models;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System.Linq;
using System.Collections.Generic;

namespace Fin_Manager_v2.Views;

public sealed partial class MonthlyViewPage : Page
{
    public MonthlyViewViewModel ViewModel
    {
        get;
    }

    public MonthlyViewPage()
    {
        ViewModel = App.GetService<MonthlyViewViewModel>();
        InitializeComponent();
        Loaded += MonthlyViewPage_Loaded;
    }

    private void MonthlyViewPage_Loaded(object sender, RoutedEventArgs e)
    {
        // Khởi tạo các giá trị mặc định khi page được load
        if (ViewModel.SelectedAccount == null && ViewModel.Accounts.Count > 0)
        {
            ViewModel.SelectedAccount = ViewModel.Accounts[0];
        }
    }

    private void OnTagSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ListBox listBox)
        {
            ViewModel.SelectedTags = listBox.SelectedItems.Cast<TagModel>().ToList();
            ViewModel.OnTagSelectionChanged(sender, e);
        }
    }

   private void Tag_CheckedChanged(object sender, RoutedEventArgs e)
    {
        // Lấy danh sách tags đã chọn từ ViewModel.AvailableTags
        var selectedTags = ViewModel.AvailableTags.Where(t => t.IsSelected).ToList();
        ViewModel.SelectedTags = selectedTags;
    }
}
