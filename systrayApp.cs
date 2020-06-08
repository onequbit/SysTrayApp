
using Library;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.ServiceProcess;
using System.Text;
using System.Windows.Forms;

namespace App
{
    public class Program
    {
        private const string optionsFileName = "options.csv";

        private List<SysTrayOption> sysTrayOptions;        
        
        private SysTrayIcon trayIcon;

        public Program()
        {
            try
            {                
                this.sysTrayOptions = optionsFileName.ReadFileAsStringList().ToSysTrayOptions();
                this.trayIcon = new SysTrayIcon(this.sysTrayOptions, showServicesStatus);                
            }
            catch (Exception ex)
            {
                ex.ToMessageBox("Program constructor failed");
            }
        }

        public void Go()
        {
            this.trayIcon.Run();
        }

        [STAThread] // required for Windows Forms
        public static void Main(string[] args)
        {               
            new Program().Go();
        }

        private void showServicesStatus(object sender, EventArgs e)
        {
            try
            {                                                     
                this.trayIcon.ShowInfoBalloon("Status:", this.sysTrayOptions.ToServiceStatusesString());
            }
            catch (Exception ex)
            {
                ex.ToMessageBox();
                EventLog.WriteEntry("SysTrayApp", ex.Message, EventLogEntryType.Error, 1, 1, new byte[1]{0});
            }
        }       
    }  

    public static partial class ExtensionMethods
    {
        public static List<string> ReadFileAsStringList(this string filename)
        {               
            List<string> data = new List<string>{};
            try
            {
                string localFilePath = Path.Combine(Environment.CurrentDirectory, filename);
                foreach(string line in File.ReadAllLines(localFilePath))
                {
                    if (!line.NullOrEmpty())
                    {
                        data.Add(line.Trim());                        
                    }        
                }                  
            }
            catch(Exception ex)
            {            
                ex.ToMessageBox($"failed to read from {filename}");
            }
            return data;        
        }
    }
}