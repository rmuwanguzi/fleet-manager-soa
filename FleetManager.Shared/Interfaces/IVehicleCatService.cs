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
    public interface IVehicleCatService : IServiceCoreSingle<vh_categoryC>
    {
        Task<vh_categoryC> AddVehicleCategory(dto_vehicle_category_newC dto);
        Task<vh_categoryC> UpdateVehicleCategory(dto_vehicle_category_updateC dto);
    }
}
