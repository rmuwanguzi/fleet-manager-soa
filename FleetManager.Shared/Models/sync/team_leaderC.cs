
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetManager.Shared.Models
{
    
    public class team_leaderC :serverC
    {
        
        public int team_leader_id { get; set; }
        public string team_leader_name { get; set; }
        public int user_id { get; set; }
        public string phone_no { get; set; }

        //
    }
}
