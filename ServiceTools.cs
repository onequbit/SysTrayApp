using System;
using System.Collections.Generic;
using System.Drawing;
using System.ServiceProcess;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;

namespace Library
{

    public class ServiceTools
    {
        public static void StopService(string name)
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            using (WindowsImpersonationContext context = identity.Impersonate())
            {
                ServiceController service = new ServiceController(name);
                service.Stop();            
                service.WaitForStatus(ServiceControllerStatus.Stopped);            
            }
        }

        public static void ToggleService(string name)
        {
            try
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                using (WindowsImpersonationContext context = identity.Impersonate())
                {
                    ServiceController service = new ServiceController(name);
                    if (service.Status == ServiceControllerStatus.Running)
                    {
                        service.Stop();            
                        service.WaitForStatus(ServiceControllerStatus.Stopped);
                        return;
                    }
                    if (service.Status == ServiceControllerStatus.Stopped)
                    {
                        service.Start();            
                        service.WaitForStatus(ServiceControllerStatus.Running);
                        return;
                    }                
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        
        public static string GetWindowsServiceStatus(string name)
        {
            string status = "";
            try
            {
                ServiceController sc = new ServiceController(name);
                switch (sc.Status)
                {
                    case ServiceControllerStatus.Running:
                        status = "Running";
                        break;
                    case ServiceControllerStatus.Stopped:
                        status = "Stopped";
                        break;
                    case ServiceControllerStatus.Paused:
                        status = "Paused";
                        break;
                    case ServiceControllerStatus.StopPending:
                        status = "Stopping";
                        break;
                    case ServiceControllerStatus.StartPending:
                        status = "Starting";
                        break;
                    default:
                        status = "Status ?...";
                        break;
                }
            }
            catch
            {
                status = "error";
            }
            return status;
        }

        public static Dictionary<string,string> GetWindowsServicesStatuses(string[] names)
        {
            Dictionary<string,string> statuses = new Dictionary<string, string>{};
            foreach (string service in names) 
            {
                statuses[service] = ServiceTools.GetWindowsServiceStatus(service);                
            }
            return statuses;
        }

        public static bool IsRunning(string name)
        {
            return ServiceTools.GetWindowsServiceStatus(name).Equals("Running");
        }

        public static Dictionary<string,string> ServiceNamesLookup
        {
            get 
            {
                Dictionary<string,string> names = new Dictionary<string,string>{};
                ServiceController[] services = ServiceController.GetServices();
                foreach (ServiceController service in services)
                {
                    names[service.ServiceName] = service.DisplayName;
                    names[service.DisplayName] = service.ServiceName;
                }
                return names;
            }
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
}
