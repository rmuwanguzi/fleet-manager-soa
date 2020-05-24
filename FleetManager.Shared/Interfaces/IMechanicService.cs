using FleetManager.Shared.dto;
using FleetManager.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetManager.Shared.Interfaces
{
    public interface IMechanicService : IServiceCore<mechanicC, dto_mechanic_newC, dto_mechanic_updateC>
    {
    }
}
