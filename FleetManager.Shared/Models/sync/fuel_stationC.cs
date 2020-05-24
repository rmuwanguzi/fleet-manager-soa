
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetManager.Shared.Models
{
    
    public class fuel_stationC : serverC
    {
        
        public int fuel_station_id { get; set; }
        public string fuel_station_name { get; set; }
        public int cr_account_id { get; set; }
        public int cr_balance_amount { get; set; }
        public string contact_person { get; set; }
        public string phone_no { get; set; }
    }
}
