
using System;
using System.Drawing;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;
using WinFormsApp = System.Windows.Forms.Application;

namespace App
{
    public class SysTrayApp : Form
    {
        private static Mutex mutex = null;

        [STAThread]
        public static void Main(string[] args)
        {
            
            bool createdNew;
            string appName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;    
            mutex = new Mutex(true, appName, out createdNew);    
            if (!createdNew)  
            {                  
                return;  
            }
            
            WinFormsApp.Run(new SysTrayApp());
        }
        private NotifyIcon  trayIcon;
        private ContextMenu trayMenu;
        private MenuItem trayExitItem;

        public SysTrayApp()
        {
            trayExitItem = new System.Windows.Forms.MenuItem();            
            trayExitItem.Index = 0;
            trayExitItem.Text = "E&xit";
            trayExitItem.Click += ExitHandler;
            
            trayMenu = new ContextMenu();            
            trayMenu.MenuItems.Add(trayExitItem);           

            trayIcon = new NotifyIcon();            
            trayIcon.Text = "double click for info";
            trayIcon.Icon = new Icon("onequbit.ico");
            trayIcon.DoubleClick  += trayIcon_DoubleClick; 
            trayIcon.ContextMenu = trayMenu;
            trayIcon.Visible     = true;
        }

        protected override void OnLoad(EventArgs e)
        {
            Visible       = false; // Hide form window.
            ShowInTaskbar = false; // Remove from taskbar. 
            base.OnLoad(e);
        }

        private void ExitHandler(object sender, EventArgs e)
        {            
            Application.Exit();
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {                
                trayIcon.Dispose();
            } 
            base.Dispose(isDisposing);
        }

        private void trayIcon_DoubleClick(object sender, EventArgs e)
        {
            trayIcon.BalloonTipIcon = ToolTipIcon.Info;
            trayIcon.BalloonTipTitle = "";
            string service = "vmcompute";
            string status = GetWindowsServiceStatus(service);
            trayIcon.BalloonTipText = $"{service} is {status}";
            trayIcon.ShowBalloonTip(8192);
        }

        private string GetWindowsServiceStatus(String serviceName)
        {
            string status = "";
            try
            {
                ServiceController sc = new ServiceController(serviceName);
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
                        status = "Status Changing";
                        break;
                }
            }
            catch
            {
                status = "unknown - service not found";
            }
            return status;
        }

    }

    // public static class ExtensionMethods
    // {
    //     public static bool SetHighDpiMode(System.Windows.Forms.HighDpiMode mode)
    //     {
    //         return false;
    //     }
    // }

}