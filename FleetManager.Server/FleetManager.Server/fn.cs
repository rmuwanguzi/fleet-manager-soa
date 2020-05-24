using System;
using Autofac;
using System.Web.Hosting;
using Service;
using FleetManager.Shared;
using FleetManager.Shared.Interfaces;
using MutticoFleet.Services;
using MutticoFleet.Server.Platform_Specific;
using MutticoFleet.Service.Interfaces;
using FleetManager.Shared.Core;
using MutticoFleet.Service;

namespace MutticoFleet.Server
{
    public static class fn
    {

    public static string CONN_STRING = "data source= ;initial catalog= ;integrated security=True";
    

        internal static void Injection()
        {

            fnn.DB_SCHEMA = "fleet_dev";
            //injection        
            fnn.DB_PROVIDER = em.db_Type.sql_server;
            var builder = new ContainerBuilder();

            builder.RegisterType<TokenService>().As<ITokenService>().SingleInstance();
            builder.RegisterType<CheckForSoftwareUpdatesService>().As<ICheckForSoftwareUpdatesService>().SingleInstance();
            builder.RegisterType<SecurityGroupService>().As<ISecurityGroupService>().SingleInstance();
            builder.RegisterType<RefreshTokenService>().As<IRefreshTokenService>().SingleInstance();
            builder.RegisterType<InitDataService>().As<IInitDataService>().SingleInstance();
            builder.RegisterType<LoginService>().As<ILoginService>().SingleInstance();
            builder.RegisterType<LoggedInUserService>().As<ILoggedInUserService>().SingleInstance();

            builder.RegisterType<ExpenseTypeService>().As<IExpenseTypeService>().SingleInstance();
            builder.RegisterType<ExpenseTypeFieldService>().As<IExpenseTypeFieldService>().SingleInstance();
            builder.RegisterType<ExpenseTypeFieldItemService>().As<IExpenseTypeFieldItemService>().SingleInstance();
            builder.RegisterType<DriverService>().As<IDriverService>().SingleInstance();
            builder.RegisterType<VehicleCatService>().As<IVehicleCatService>().SingleInstance();
            builder.RegisterType<VehicleService>().As<IVehicleService>().SingleInstance();
            builder.RegisterType<ExpenseService>().As<IExpenseService>().SingleInstance();
            builder.RegisterType<UserService>().As<IUserService>().SingleInstance();
            builder.RegisterType<TeamLeaderService>().As<ITeamLeaderService>().SingleInstance();
            builder.RegisterType<ProjectService>().As<IProjectService>().SingleInstance();
            builder.RegisterType<VehicleOwnerService>().As<IVehicleOwnerService>().SingleInstance();
            builder.RegisterType<MechanicService>().As<IMechanicService>().SingleInstance();
            builder.RegisterType<FuelStationService>().As<IFuelStationService>().SingleInstance();
            builder.RegisterType<CreditorService>().As<ICreditorService>().SingleInstance();



            //
            Platform_Specific.MessageDialog _messaging = new Platform_Specific.MessageDialog();
            builder.RegisterInstance(_messaging).As<IMessageDialog>().SingleInstance();


            Platform_Specific.SqlServerDbContextHelper _dbContextHelper = new Platform_Specific.SqlServerDbContextHelper();
            builder.RegisterInstance(_dbContextHelper).As<IDbContextHelper>().SingleInstance();

            Platform_Specific.PCLSettings _settings = new Platform_Specific.PCLSettings();
            _settings.BaseUrl = "https://api.com/";
            if (!HostingEnvironment.IsDevelopmentEnvironment)
            {
                _settings.isDevEnviroment = false;
                _settings.errorLogRootFolderPath = HostingEnvironment.MapPath("~/ErrorLogs/");
               
            }
            else
            {
                _settings.isDevEnviroment = true;
                _settings.errorLogRootFolderPath = HostingEnvironment.MapPath("~/App_Data/");
            }
            builder.RegisterInstance(_settings).As<IPCLSettings>().SingleInstance();
            try
            {
                ObjectBase._container = builder.Build();

            }
            catch (Exception ex)
            {
                throw ex;
            }



        }
    }
}