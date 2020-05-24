
using FleetManager.Shared.Interfaces;

namespace MutticoFleet.Server.Platform_Specific
{
    public class PCLSettings : IPCLSettings
    {
        public string BaseUrl
        {
            get; set;
        }
        public string DB_PATH { get; set; }
        public string controller_key { get; set; }
        public string errorLogRootFolderPath { get; set; }
        public bool isDevEnviroment { get; set; }
    }
}