using FleetManager.Shared;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetManager.Shared.Models
{
   
   public class vh_expense_transC : serverC
    {
        
        public int expense_id { get; set; }
        public DateTime expense_date { get; set; }
        public int expense_fs_id { get; set; }
        public int voucher_id { get; set; }
        public string voucher_no { get; set; }
        public em.vehicle_expense_categoryS exp_type_cat
        {
            get
            {
                return (em.vehicle_expense_categoryS)exp_type_cat_id;
            }
        }
        public int exp_type_cat_id { get; set; }
        public int exp_type_id { get; set; }
        public string exp_type_name { get; set; }
        public int expense_amount { get; set; }
        public string expense_details { get; set; }
        public int m_partition_id { get; set; }
        public int vehicle_id { get; set; }
        public string vehicle_plate_no { get; set; }
        public int driver_id { get; set; }
        public string driver_name { get; set; }
        public int vehicle_cat_id { get; set; }
        public string json_extra_fields_info { get; set; }
        public int repair_action_type_id { get; set; }
        public string repair_action_type_name { get; set; }
        public int user_id { get; set; }
        public string user_name { get; set; }
        public int team_leader_id { get; set; }
        public string team_leader_name { get; set; }
        public int vh_owner_id { get; set; }
        public string vh_owner_name { get; set; }
        public int project_id { get; set; }
        public string project_name { get; set; }
        public string fuel_station_name { get; set; }
        public int fuel_station_id { get; set; }
        public int cr_invoice_id { get; set; }
        public int mechanic_id { get; set; }//optional
        public string mechanic_name { get; set; }
    }
}
