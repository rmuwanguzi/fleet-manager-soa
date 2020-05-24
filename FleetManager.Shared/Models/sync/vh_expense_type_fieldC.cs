using FleetManager.Shared;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetManager.Shared.Models
{
    
    public class vh_expense_type_fieldC : serverC
    {
        
        public int et_field_id { get; set; }
        public string et_field_name { get; set; }
        public int exp_type_id { get; set; }
        public em.field_typeS et_field_type
        {
            get
            {
                return (em.field_typeS)this.et_field_type_id;
            }
        }
        public int et_field_type_id
        {
            get;set;
        }
    }
}
