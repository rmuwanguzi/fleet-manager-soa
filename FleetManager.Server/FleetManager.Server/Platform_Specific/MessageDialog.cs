using FleetManager.Shared.Interfaces;


namespace MutticoFleet.Server.Platform_Specific
{
    public class MessageDialog : IMessageDialog
    { 
       
        public void ErrorMessage(string message, string title = null)
        {
            return;
        }
        public object tag { get; set; }
        public void ErrorMessage(string key, string message, string controller_key = null)
        {
            if (!string.IsNullOrWhiteSpace(controller_key))
            {
                var modelState = datam.DATA_CONTROLLER_MODEL_STATE[controller_key];
                modelState.AddModelError(controller_key, message);
            }
         }
    }
}