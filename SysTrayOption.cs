
using Library;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Text;
using System.Windows.Forms;

namespace Library
{
    public enum OptionTypeEnum
    {
        Service,
        Command
    }

    public class SysTrayOption
    {        
        public bool Error = false;
        public OptionTypeEnum OptionType;

        public bool IsService
        {
            get
            {
                return this.OptionType == OptionTypeEnum.Service;
            }
        }

        public ServiceController Service { get; set; }

        public string ServiceName
        {
            get
            {
                return (this.IsService)? this.Service.ServiceName : "";                
            }
        }

        private string _displayName;
        public string DisplayName
        {
            get
            {
                return (this.IsService)? this.Service.DisplayName : this._displayName;
            }
            set
            {
                if (!this.IsService) this._displayName = value;
            }
        }

        public string Status
        {
            get 
            {
                return (this.IsService)? this.Service.Status.ToString() : "";
            }
        }

        public string ServiceStatus
        {
            get 
            {
                if (!this.IsService) return "";
                string name = this.ServiceName;
                string status = this.Status;
                return $"{name} is {status}";
            }
        }

        public string ServiceOptionText
        {
            get
            {
                if (!this.IsService) return "";
                string name = this.ServiceName;
                string status = this.Status;                
                string serviceToggle = ServiceTools.TOGGLE[this.Status];
                return $"{name} : {serviceToggle}";
            }            
        }

        public string CommandStr { get; set; }

        public SysTrayIcon ParentIcon { get; set; }

        public SysTrayOption(string optionString)
        {
            try
            {
                if (ServiceTools.CheckIsService(optionString))
                {
                    initAsService(optionString);
                }
                else
                {
                    initAsCommand(optionString);
                }
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex, $"failed to create SysTrayOption {optionString}");
                throw;
            }
        }

        private void initAsService(string optionString)
        {
            try
            {
                string service = optionString.NoParens();
                                
                this.Service = new ServiceController(service);
                this.OptionType = OptionTypeEnum.Service;
                this.CommandStr = "";
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex, $"failed to attach Service '{optionString}' to SysTrayOption()");
                throw;
            }
        }

        private void initAsCommand(string optionString)
        {
            try 
            {                
                string[] parts = optionString.Split(',');
                if (parts.Length != 2)
                    throw new Exception($"unable to parse command option from '{optionString}'");
                this.OptionType = OptionTypeEnum.Command;                    
                this.DisplayName = parts[0].NoParens();
                if (parts[1].Split(' ').Length == 1) this.CommandStr = parts[1].NoParens();
                    else this.CommandStr = parts[1];
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex, $"failed to attach Command '{optionString}' to SysTrayOption()");
                throw;
            }            
        }

        public MenuItem ToMenuItem()
        {
            MenuItem item = new MenuItem();
            
            if (OptionType == OptionTypeEnum.Service)
            {                
                item.Text = this.ServiceOptionText;
                item.Click += (o,e) => TryServiceToggle();
            }
            else
            {
                item.Text = this.DisplayName;
                Process proc = new Process().ToRunAsAdmin(this.CommandStr);                
                item.Click += (object o, EventArgs e) => proc.Start();
            }             
            item.Enabled = this.Status != "error";
            return item;
        }

        public void UpdateMenuItem()
        {

        }

        private void TryServiceToggle()
        {
            this.Error = !this.Service.ToggleStatus();
            if (this.Error) 
            {
                this.ParentIcon.ShowInfoBalloon("Error:", "this.Service.ToggleStatus() failed");
            } else
            {
                this.ParentIcon.ShowInfoBalloon(this.DisplayName, this.ServiceStatus);
            }           
        }

        public override string ToString()
        {
            string[] lines = new string[] { "{",
                $"OptionType: {this.OptionType}",
                $"ServiceName: {this.ServiceName}",
                $"DisplayName: {this.DisplayName}",
                $"CommandStr: {this.CommandStr}","}"
            };
            return string.Join("\n", lines);
        }

        private void ErrorHandler(Exception ex, string message)
        {
            if (this.ParentIcon != null)
            {
                this.ParentIcon.ShowInfoBalloon("Error:", message);
            }
            else 
            {
                ex.ToMessageBox(message);
            }
        }         
    }

    public static partial class ExtensionMethods
    {
        public static void ToMessageBox(this SysTrayOption option, string name = "", bool enableExit = true)
        {
            showMessageWithOptions(option.ToString(), name, enableExit);            
        }

        public static void ToMessageBox(this SysTrayOption[] options, string name = "", bool enableExit = true)
        {
            List<string> newList = new List<string>{};                
            foreach(SysTrayOption option in options)
            {
                newList.Add(option.ToString());                    
            }
            newList.ToArray().ToMessageBox(name, enableExit);
        }

        public static List<SysTrayOption> ToSysTrayOptions(this IEnumerable<string> options)
        {
            List<SysTrayOption> newSTOptions = new List<SysTrayOption>(){};        
            foreach(string option in options)
            {
                newSTOptions.Add(new SysTrayOption(option));                
            }
            return newSTOptions;            
        }

        public static List<MenuItem> ToMenuItems(this List<SysTrayOption> options)
        {
            List<MenuItem> items = new List<MenuItem>(){};        
            foreach(SysTrayOption option in options)
            {
                items.Add(option.ToMenuItem());                
            }
            return items;            
        }
        
        public static string ToServiceStatusesString(this IEnumerable<SysTrayOption> options)
        {
            StringBuilder lines = new StringBuilder();              
            foreach(SysTrayOption option in options)
            {
                lines.AppendLine(option.ServiceStatus);                            
            }
            return lines.ToString();            
        }

        public static void AttachToParent(this IEnumerable<SysTrayOption> options, SysTrayIcon parent)
        {
            foreach(SysTrayOption option in options)
            {
                option.ParentIcon = parent;
            }
        }
    }
}