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
    public interface IDriverService : IServiceCore<vh_driverC, dto_driver_newC, dto_driver_updateC>
    {
    }
}
