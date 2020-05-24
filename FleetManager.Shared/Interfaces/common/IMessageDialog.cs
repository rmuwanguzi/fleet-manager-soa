using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetManager.Shared.Interfaces
{
    public interface IMessageDialog
    {
    
        void ErrorMessage(string message, string title = null);
        void ErrorMessage(string key, string message, string controller_key=null);
         
    }
}
