
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetManager.Shared.Models
{
    
    public class projectC : serverC
    {
        public string project_name { get; set;}
        
        public int project_id { get; set;}
        public int no_of_fuel_stations { get; set; }

    }
}
