

using System.Threading.Tasks;
using System.Collections.Generic;
using FleetManager.Shared.Interfaces;
using FleetManager.Shared.dto;

namespace FleetManager.Shared.Interfaces
{
    public interface IUserService : IService
    {
        Task<dto_pc_userC> AddNewUser(dto_pc_user_newC _dto);
        Task<List<dto_pc_userC>> GetAllUsers(long fs_timestamp);
        Task<bool> DeleteUser(int id);
        Task<bool> UpdateUser(dto_pc_user_updateC _dto);
        Task<bool> ChangeUserPwd(dto_pc_user_change_password _dto);
        Task<bool> ResetPassword(int user_id);
      
    }
}
