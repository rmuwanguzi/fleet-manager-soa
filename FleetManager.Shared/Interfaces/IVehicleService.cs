using FleetManager.Shared.Models;
using FleetManager.Shared.dto;
using FleetManager.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetManager.Shared.Interfaces
{
   public  interface IVehicleService :IServiceCore<vehicleC, dto_vehicle_newC, dto_vehicle_updateC>, IService
    {
      
       
    }
}
