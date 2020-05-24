using FleetManager.Shared.dto;
using FleetManager.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetManager.Shared.Interfaces
{
    public interface ITeamLeaderService : IServiceCore<team_leaderC, dto_team_leader_newC, dto_team_leader_updateC>
    {
    }
}
