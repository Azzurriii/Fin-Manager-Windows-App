using Fin_Manager_v2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// View model for displaying details of an account.
/// </summary>
/// <remarks>
/// This class represents the view model for displaying the details of an account.
/// </remarks>
namespace Fin_Manager_v2.ViewModels;

public class AccountDetailViewModel
{
    public AccountModel Account { get; set; }

    public AccountDetailViewModel(AccountModel account)
    {
        Account = account;
    }
}
