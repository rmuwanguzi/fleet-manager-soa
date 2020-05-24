
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetManager.Shared.Models
{
    
    public class security_groupC : serverC
    {
        
        public int sec_group_id { get; set; }
        public string sec_group_name { get; set; }//*pk
        public string sec_group_rights_ids { get; set; }
        public int no_of_users { get; set; }
        public int no_of_rights { get; set; }
        public int sec_group_status_id { get; set; }
        public em.security_group_statusS sec_group_status
        {
            get
            {
                return (em.security_group_statusS)sec_group_status_id;
            }
        }
        public security_groupC()
        {
            sec_group_status_id = Convert.ToInt32(em.security_group_statusS.enabled);
        }
    }
}
