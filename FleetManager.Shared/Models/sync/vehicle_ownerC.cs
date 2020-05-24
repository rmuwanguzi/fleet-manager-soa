
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetManager.Shared.Models
{
    
    public class vehicle_ownerC : serverC
    {
        public string vh_owner_name { get; set; }
        
        public int vh_owner_id { get; set; }
        public string vh_owner_phone_no { get; set;}
        public int cr_account_id { get; set; }
        public int cr_balance_amount { get; set; }
    }
}
