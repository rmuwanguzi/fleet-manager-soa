using FleetManager.Shared.dto;
using FleetManager.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetManager.Shared.Interfaces
{
    public interface ILoginService : IService
    {
        Task<dto_pc_userC> LoginAdmin(dto_login _dto);
        

    }
}
