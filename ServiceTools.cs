using System;
using System.Collections.Generic;
using System.Drawing;
using System.ServiceProcess;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;
using scs = System.ServiceProcess.ServiceControllerStatus;

namespace Library
{

    public static class ServiceTools
    {
        public static ServiceController[] AllServices
        {
            get
            {
                return ServiceController.GetServices();
            }
        }

        public static string[] ServiceNames
        {
            get
            {
                return ServiceTools.AllServices.ToNames();
            }            
        }

        public static bool CheckIsService(string name)        
        {
            return ServiceTools.ServiceNames.Contains(name);            
        }

        public static Dictionary<string,string> TOGGLE 
        {
            get {
                return new Dictionary<string, string>
                {
                    ["Running"] = "Stop",
                    ["Stopped"] = "Start",
                    ["error"] = "error"
                };
            }
        }      
    }

    public static partial class ExtensionMethods
    {                 
        public static string ToString(this scs status)
        {            
            switch (status)
            {
                case scs.Stopped : return "Stopped";
                case scs.StartPending : return "StartPending";
                case scs.StopPending : return "Stopped";
                case scs.Running : return "StartPending";
                case scs.ContinuePending : return "Stopped";
                case scs.PausePending : return "StartPending";
                case scs.Paused : return "Stopped";                
                default: return "error";
            }
        }

        public static bool IsRunning(this ServiceController service)
        {
            return service.Status == scs.Running;
        }

        public static bool IsStopped(this ServiceController service)
        {
            return service.Status == scs.Stopped;
        }

        public static bool ToggleStatus(this ServiceController service)
        {            
            try
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                using (WindowsImpersonationContext context = identity.Impersonate())
                {                
                    if (service.IsRunning())
                    {
                        service.Stop();            
                        service.WaitForStatus(scs.Stopped);
                        return true;
                    }                     
                    if (service.IsStopped())
                    {
                        service.Start();            
                        service.WaitForStatus(scs.Running);
                        return true;
                    }                
                    
                } 
            }
            catch { }
            return false;
        }
    } 
}
