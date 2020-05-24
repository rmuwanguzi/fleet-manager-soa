
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetManager.Shared.Models
{
   
   public class creditorC : serverC
    {
        
        public int cr_account_id { get; set; }
        public int cr_balance { get; set; }
        public string cr_account_name { get; set; }
        public string cr_phone_no { get; set; }
        public em.creditor_typeS cr_account_type
        {
            get
            {
                return (em.creditor_typeS)cr_account_type_id;
            }
        }
        public int cr_account_type_id
        {
            get;set;
        }
        public int opening_cr_balance { get; set; }
        public DateTime? opening_cr_balance_start_date { get; set; }
        public int cr_owner_id { get; set; }
        public int last_cr_deposit_id { get; set; }
        public DateTime? last_pay_date { get; set; }
    }
}
