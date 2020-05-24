
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetManager.Shared.Models
{
    
    public class mechanicC : serverC
    {
        public string mechanic_name { get; set;}
        public string phone_no { get; set;}
        
        public int mechanic_id { get; set;}
        public int cr_account_id { get; set; }
        public int cr_balance_amount { get; set; }
    }
}
