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

namespace Library
{
    
    public class SysTrayIcon : Form
    {                
        
        private NotifyIcon  trayIcon;

        private ContextMenu trayMenu;
        
        private string[] serviceNames;

        private SysTrayOption[] OptionsList;

        private Dictionary<string, string> serviceStatuses;

        public SysTrayIcon(SysTrayOption[] optionsList)
        {            
            OptionsList = optionsList;
            serviceNames = OptionsList.GetServiceNames();
            serviceStatuses = ServiceTools.GetWindowsServicesStatuses(serviceNames);
            trayMenu = initContextMenu();           
            trayIcon = initNotifyIcon();                        
        }

        private ContextMenu initContextMenu()
        {
            ContextMenu menu = new ContextMenu();
            menu.MenuItems.AddRange(buildMenuItems(OptionsList));
            menu.Popup += popupHandler; 
            return menu;
        }

        private NotifyIcon initNotifyIcon()
        {
            NotifyIcon icon = new NotifyIcon()
            {
                Text = "double click for info",
                Icon = this.GetCurrentIcon(),              
                ContextMenu = trayMenu,
                Visible = true
            };
            icon.DoubleClick += showServicesStatus;
            return icon;
        }

        private MenuItem[] buildMenuItems(SysTrayOption[] options)
        {
            List<MenuItem> items = new List<MenuItem>{};
            MenuItem defaultExit = new MenuItem("E&xit", exitHandler);
            defaultExit.DefaultItem = true;
            items.Add(defaultExit);            
            foreach(SysTrayOption option in options)
            {
                MenuItem item = option.ToMenuItem();
                if (option.isService)
                {
                    item.Click += showServicesStatus;
                }
                items.Add(item);
            }
            return items.ToArray();
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
            Application.Exit();
        }

        private void popupHandler(object o, EventArgs e)
        {
            trayMenu.MenuItems.Clear();
            trayMenu.MenuItems.AddRange(buildMenuItems(OptionsList));
        }

        private void showServicesStatus(object sender, EventArgs e)
        {
            bool cancelEvent = OptionsList.CancelOnError();
            if (cancelEvent) return;

            string statusText = "";
            serviceStatuses = OptionsList.GetServiceStatuses();
            foreach (string service in serviceNames)
            {
                string status = serviceStatuses[service];
                statusText += $"{service} is {status}\n";
            }                        
            trayIcon.ShowInfoBalloon(statusText);
        }
    }
}