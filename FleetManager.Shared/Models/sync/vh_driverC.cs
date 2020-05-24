
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetManager.Shared.Models
{
   
   public class vh_driverC : serverC
    {
        
        public int driver_id { get; set; }
        public string driver_name { get; set; }
        public string driver_phone_no { get; set; }
        public string permit_no { get; set; }
        public DateTime? first_issue_date { get; set; }
        public DateTime? renew_issue_date { get; set; }
        public DateTime? renew_expiry_date { get; set; }
        public string permit_class_ids { get; set; }
    }
}
