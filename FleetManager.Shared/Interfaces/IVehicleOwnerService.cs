using FleetManager.Shared.dto;
using FleetManager.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetManager.Shared.Interfaces
{
    public interface IVehicleOwnerService : IServiceCore<vehicle_ownerC, dto_vehicle_owner_newC, dto_vehicle_owner_updateC>
    {
    }
}
