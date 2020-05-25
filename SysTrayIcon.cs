using Library;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormsApp = System.Windows.Forms.Application;

namespace App
{
    
    public class SysTrayIcon : Form
    {        
        private bool appRunning;
        private NotifyIcon  trayIcon;

        private const int BALLOON_TIMEOUT = 8192;

        private Dictionary<string,string> TOGGLE 
        {
            get {
                return new Dictionary<string, string>
                {
                    ["Running"] = "Stop",
                    ["Stopped"] = "Start"
                };
            }
        }

        private ContextMenu trayMenu;
        
        private string[] serviceNames;

        private Dictionary<string, string> serviceStatuses;

        public SysTrayIcon(string[] services)
        {
            appRunning = true;
            serviceNames = services;
            serviceStatuses = ServiceTools.GetWindowsServicesStatuses(serviceNames);
            Task.Run(()=>serviceMonitorLoop());

            trayMenu = new ContextMenu();            
            trayMenu.MenuItems.Add(new MenuItem("E&xit", exitHandler));
            trayMenu.MenuItems[0].DefaultItem = true;
            trayMenu.MenuItems.AddRange(buildMenuItems(serviceNames));
            trayMenu.Popup += popupHandler;

            trayIcon = new NotifyIcon();            
            trayIcon.Text = "double click for info";
            trayIcon.Icon = new Icon("onequbit.ico");
            trayIcon.DoubleClick  += showServicesStatus; 
            
            trayIcon.ContextMenu = trayMenu;
            trayIcon.Visible     = true;

        }

        private MenuItem[] buildMenuItems(string[] names)
        {
            List<MenuItem> items = new List<MenuItem>{};
            foreach(string name in names)
            {
                string menuOption = setServiceOption(name);
                items.Add(new MenuItem(menuOption, (object o,EventArgs e) => ServiceTools.ToggleService(name)));
            }
            return items.ToArray();
        }

        private string setServiceOption(string serviceName)
        {
            string serviceToggle = TOGGLE[serviceStatuses[serviceName]];
            return $"{serviceName} : {serviceToggle}";
        }

        private void updateMenuOptions()
        {
            foreach(MenuItem item in trayMenu.MenuItems)
            {
                string name = item.Text.Split(' ')[0];
                if (serviceNames.Contains(name))                
                    item.Text = setServiceOption(name);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            Visible       = false; // Hide form window.
            ShowInTaskbar = false; // Remove from taskbar. 
            base.OnLoad(e);
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {                
                trayIcon.Dispose();
            } 
            base.Dispose(isDisposing);
        }

        private void exitHandler(object o, EventArgs e)
        {
            appRunning = false;
            Application.Exit();
        }

        private void popupHandler(object o, EventArgs e)
        {
            updateMenuOptions();
        }

        private void serviceMonitorLoop()
        {
            while (appRunning)
            {
                serviceStatuses = ServiceTools.GetWindowsServicesStatuses(serviceNames);
                Thread.Sleep(1000);
            }
        }

        private void showServicesStatus(object sender, EventArgs e)
        {            
            string statusText = "";
            foreach (string service in serviceNames)
            {
                string status = serviceStatuses[service];
                statusText += $"{service} is {status}\n";
            }                        
            trayIcon.ShowInfoBalloon(statusText);
        }
    }
}