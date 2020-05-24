using FleetManager.DbBase;
using MutticoFleet.Server;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;


namespace MutticoFleet.Server
{
    public class DbContext : BaseContext
    {
        public DbContext() : base(new SqlConnection(fn.CONN_STRING), true, "fleet_dev")
        {
            
        }
    }
}