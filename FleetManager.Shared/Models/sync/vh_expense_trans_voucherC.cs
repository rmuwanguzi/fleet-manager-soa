
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetManager.Shared.Models
{
    
    public class vh_expense_trans_voucherC : serverC
    {
        
        public int voucher_id { get; set; }
        public string voucher_no { get; set; }
        public int vehicle_id { get; set; }
        public string vehicle_plate_no { get; set; }
        public int driver_id { get; set; }
        public string driver_name { get; set; }
        public int voucher_total_amount { get; set; }
        public int user_id { get; set; }
        public string user_name { get; set; }
        public DateTime? expense_date { get; set; }
        public int expense_fs_id { get; set; }
        public int m_partition_id { get; set; }



    }
}
