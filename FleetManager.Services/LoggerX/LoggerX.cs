using Autofac;
using KellermanSoftware.NetLoggingLibrary;
using KellermanSoftware.NetLoggingLibrary.Targets;
using FleetManager.Shared.Interfaces;
using FleetManager.Shared.Core;
using System.IO;

namespace MutticoFleet.Service
{
   internal class LoggerX
    {
        private static bool is_init = false;
        private static void initLogger()
        {
            if (is_init) { return; }
            //  var rootPath = HostingEnvironment.MapPath("~/ErrorLogs/");
            if (ObjectBase._container == null)
            {
                Log.ResetConfiguration();
                var _log_4VUdp_target = new Log4ViewUdpTarget();
                _log_4VUdp_target.CurrentIpAddress = "127.0.0.1";
                _log_4VUdp_target.Port = 878;
                _log_4VUdp_target.MinimumLevel = LoggingLevel.ERROR;
                _log_4VUdp_target.Mode = LoggingMode.Asynchronous;
                Log.Config.Targets.Add(_log_4VUdp_target);
                               
                is_init = true;
                return;
            }
           Log.ResetConfiguration();
            var _settings = ObjectBase._container.Resolve<IPCLSettings>();
            var rootPath = _settings.errorLogRootFolderPath;
            if (!string.IsNullOrEmpty(rootPath))
            {
                //
                string _file_name = "errorLog.txt";
                var _errorFilePath = Path.Combine(rootPath, Path.GetFileName(_file_name));
                FileTarget errorTarget = new FileTarget(_errorFilePath);
                errorTarget.MinimumLevel = LoggingLevel.ERROR;
                errorTarget.Mode = LoggingMode.Asynchronous;
                //
                Log.Config.Targets.Add(errorTarget);
                //
                DebugTarget target = new DebugTarget();
                target.Mode = LoggingMode.Asynchronous;
                Log.Config.Targets.Add(target);
                if(!_settings.isDevEnviroment)
                {
                    //production
                    //send email
                }
                else
                {
                   
                    var _log_4VUdp_target = new Log4ViewUdpTarget();
                    _log_4VUdp_target.CurrentIpAddress = "127.0.0.1";
                    _log_4VUdp_target.Port = 878;
                    _log_4VUdp_target.MinimumLevel = LoggingLevel.ERROR;
                    _log_4VUdp_target.Mode = LoggingMode.Asynchronous;
                    Log.Config.Targets.Add(_log_4VUdp_target);

                   

                }
            }
            else
            {
                DebugTarget target = new DebugTarget();
                target.Mode = LoggingMode.Asynchronous;
                Log.Config.Targets.Add(target);
            }
            
            is_init = true;
        }
        
        public static void LogException(System.Exception ex)
        {
            if(fnn.isSeeding)
            {
               
                throw new System.Exception(ex.Message, ex);
                return;
            }
            initLogger();
            System.Diagnostics.Debug.WriteLine("<<ERROR>>");
            System.Diagnostics.Debug.WriteLine(ex.ToString());
            if (is_init)
            {
                Log.LogException(ex);
            }
            
        }
    }
}
