using FleetManager.Shared;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetManager.Shared.Models
{
    
   public class vh_expense_typeC : serverC
    {
        
        public int exp_type_id { get; set; }
        public string exp_type_name { get; set; }
        public int exp_cat_id { get; set; }
        public int field_count { get; set; }
        public em.vehicle_expense_categoryS exp_cat
        {
            get
            {
               return (em.vehicle_expense_categoryS)exp_cat_id;
            }
        }
    }
}
