using FleetManager.Shared.dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetManager.Shared.Interfaces
{
    public interface IProjectService : IServiceCore<Models.projectC, dto_project_newC, dto_project_updateC>
    {
        Task<Models.assign_project_fuel_stationC> AssignFuelStationToProject(dto.dto_project_assign_fuelstationC dto);
        Task<bool> RemoveFuelStationFromProject(dto.dto_project_remove_fuelstationC dto);
        Task<List<Models.assign_project_fuel_stationC>> GetFuelStationsAssignedToProjects(long fs_timestamp);
    }
}
