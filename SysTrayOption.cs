
using Library;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace Library
{
    public class SysTrayOption
    {        
        public string MenuText;
        public enum OptionTypeEnum
        {
            Service,
            Command
        }

        public bool Error = false;

        public OptionTypeEnum OptionType;

        public string ServiceName;

        public string DisplayName;

        public string ServiceStatus
        {
            get 
            {
                if (this.OptionType != OptionTypeEnum.Service) return string.Empty;
                return ServiceTools.GetWindowsServiceStatus(this.ServiceName);
            }
        }

        public bool isService
        {
            get
            {
                return this.OptionType == OptionTypeEnum.Service;
            }
        }

        public string CommandStr;

        public SysTrayOption(string optionName, string detail = "")
        {            
            if (optionName.NullOrEmpty())
                throw new SysTrayOptionException("option name invalid");

            if (isServiceName(optionName))
            {
                OptionType = OptionTypeEnum.Service;
                ServiceName = optionName;
                DisplayName = ServiceTools.ServiceNamesLookup[optionName];
                CommandStr = "";                
            } else
            {
                OptionType = OptionTypeEnum.Command;
                ServiceName = "";
                DisplayName = optionName;
                CommandStr = detail;                
            }
        }

        public MenuItem ToMenuItem()
        {
            MenuItem item = new MenuItem();
            
            if (OptionType == OptionTypeEnum.Service)
            {                
                item.Text = this.GetServiceOptionText();
                item.Click += (o,e) => TryServiceToggle();
            }
            else
            {
                item.Text = this.DisplayName;
                Process proc = new Process().ToRunAsAdmin(this.CommandStr);                
                item.Click += (object o, EventArgs e) => proc.Start();
            }            
            item.Enabled = this.ServiceStatus != "error";
            return item;
        }

        private void TryServiceToggle()
        {
            try
            {
                ServiceTools.ToggleService(this.ServiceName);
            }
            catch (Exception ex)
            {
                this.Error = true;
                MessageBox.Show(ex.Message);
            }            
        }

        private static bool isServiceName(string str)
        {            
            return ServiceTools.ServiceNamesLookup.ContainsKey(str);
        }

        public string GetServiceOptionText()
        {
            string status = this.ServiceStatus;            
            string serviceToggle = ServiceTools.TOGGLE[status];
            return $"{this.ServiceName} : {serviceToggle}";
        }
        
    }

    public class SysTrayOptionException : Exception
    {
        public SysTrayOptionException(string message) : base(message)
        { 
        }
    }

    public static partial class ExtensionMethods
    {
        public static string[] GetServiceNames(this SysTrayOption[] optionsList)
        {
            List<string> names = new List<string>{};
            foreach(SysTrayOption option in optionsList)
            {
                if (option.isService)
                {
                    names.Add(option.ServiceName);                    
                }
            }
            return names.ToArray();
        }

        public static Dictionary<string, string> GetServiceStatuses(this SysTrayOption[] optionsList)
        {
            Dictionary<string, string> statuses = new Dictionary<string,string>{};
            foreach(SysTrayOption option in optionsList)
            {
                if (option.isService)
                {
                    statuses[option.ServiceName] = option.ServiceStatus;
                }
            }
            return statuses;
        }

        public static bool CancelOnError(this SysTrayOption[] options)
        {
            for (int i=0; i<options.Length; i++)
            {
                if (options[i].Error)
                {
                    options[i].Error = false;
                    return true;
                } 
            }
            return false;
        }
    }
}