
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetManager.Shared.Models
{
   
   public class vehicleC : serverC
    {
        
        public int vehicle_id { get; set; }
        public string vehicle_plate_no { get; set; }
        public bool is_hired
        {
            get
            {
                return is_hired_id == 0 ? false : true;
            }
        }
        public int is_hired_id { get; set; }
        public int hire_amount { get; set; }
        public int driver_id { get; set; }
        public int vehicle_cat_id { get; set; }
        public int  vh_owner_id { get; set; }//0 optional
        public em.vehicle_purpose_typeS vh_purpose_type
        {
            get
            {
                return (em.vehicle_purpose_typeS)vh_purpose_type_id;
            }
        }
        public int vh_purpose_type_id { get; set; }
    }
}
