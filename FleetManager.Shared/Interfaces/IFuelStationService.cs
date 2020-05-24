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
    public interface IFuelStationService : IServiceCore<fuel_stationC, dto_fuel_station_newC, dto_fuel_station_updateC>
    {

    }
}
