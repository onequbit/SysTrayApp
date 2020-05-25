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
                status = "unknown - service not found";
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
    }

    public static class ExtensionMethods
    {
        private static Mutex mutex = null;

        public static void SingleInstance(this System.Windows.Forms.Form formApp)
        {
            bool createdNew;
            string appName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;    
            mutex = new Mutex(true, appName, out createdNew);    
            if (!createdNew)  
            {                  
                formApp.Close();
            }
        }

        public static Icon GetCurrentIcon(this System.Windows.Forms.Form formApp)
        {
            return Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        public static void Run(this System.Windows.Forms.Form app)
        {
            System.Windows.Forms.Application.Run(app);
        }

        public static void ShowInfoBalloon(this NotifyIcon icon, string message, int timeout = 8192)
        {
            icon.BalloonTipIcon = ToolTipIcon.Info;
            icon.BalloonTipTitle = "";               
            icon.BalloonTipText = message;
            icon.ShowBalloonTip(timeout);
        }
    }
}
