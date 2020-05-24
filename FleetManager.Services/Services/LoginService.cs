using System;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.Common;
using FleetManager.Shared.Interfaces;
using FleetManager.Shared.dto;
using FleetManager.Shared.Models;
using Service;
using FleetManager.Shared;
using FleetManager.Shared.Core;
using MutticoFleet.Service;

namespace MutticoFleet.Services
{
    public class LoginService : ILoginService
    {
        public string controller_key { get; set; }
        public string session_key { get; set; }
        private IMessageDialog _dialog;
        private ITokenService _tokenService;
        public LoginService(IMessageDialog dialog, ITokenService tokenService)
        {
            _dialog = dialog;
            _tokenService = tokenService;
            Task.Factory.StartNew(() =>
            {
                session_key = Guid.NewGuid().ToString();
            });
        }
        private void AddErrorMessage(string error_key, string _title, string _error_message)

        {
            if (!string.IsNullOrEmpty(controller_key))
            {
                _dialog.ErrorMessage(error_key, _error_message, controller_key);
            }
            else
            {
                _dialog.ErrorMessage(_error_message, "Save Error");
            }
        }
        public Task<dto_pc_userC> LoginAdmin(dto_login _dto)
        {
            dto_pc_userC _ret_dto_obj = null;
           
            return Task.FromResult(_ret_dto_obj);
        }
    }
}
