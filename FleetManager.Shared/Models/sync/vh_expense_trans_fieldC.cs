
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetManager.Shared.Models
{
    
    public class vh_expense_trans_fieldC : serverC
    {
        public string field_name { get; set; }
        public string field_value { get; set; }
        public int expense_id { get; set; }
        public int exp_type_id { get; set; }
        public int et_field_id { get; set; }//mileage
        public int et_field_item_id { get; set; }
        
        public int un_id { get; set; }
        public int voucher_id { get; set; }
    }
}
