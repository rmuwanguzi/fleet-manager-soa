using FleetManager.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FleetManager.Shared.dto;
using MutticoFleet.Service;

namespace MutticoFleet.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        public string controller_key { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Task<dto_refresh_tokenC> GetRefreshToken()
        {
            throw new NotImplementedException();
        }
    }
}
