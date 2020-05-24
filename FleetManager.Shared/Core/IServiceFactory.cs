using FleetManager.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FleetManager.Shared.Core
{
    public interface IServiceFactory
    {
       
       T GetService<T>() where T : class, IService;
    }
   
}
