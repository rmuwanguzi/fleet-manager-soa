

namespace FleetManager.Shared.Interfaces
{
   public interface IPCLSettings
    {
        string BaseUrl { get; set; }
        string errorLogRootFolderPath { get; set; }
        bool isDevEnviroment { get; set; }
    }
}
