using Fin_Manager_v2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fin_Manager_v2.ViewModels
{
    public class AccountDetailViewModel
    {
        public Account Account { get; set; }

        public AccountDetailViewModel(Account account)
        {
            Account = account;
        }
    }
}
