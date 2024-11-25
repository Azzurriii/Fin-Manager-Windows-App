using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fin_Manager_v2.DTO
{
    public class UpdateFinanceAccountDto
    {
        public int account_id { get; set; }
        public string account_name { get; set; }
        public string account_type { get; set; }
        public decimal initial_balance { get; set; }
        public decimal current_balance { get; set; }
        public string currency { get; set; }
    }
}
