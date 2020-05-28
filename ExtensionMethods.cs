using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.ServiceProcess;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;

public static partial class ExtensionMethods
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

    public static bool NullOrEmpty(this string s)
    {
        return s == null || s == "";
    }

    public static Process ToRunAsAdmin(this Process p, string commandStr)
    {
        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
        // startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
        startInfo.FileName = "cmd.exe";
        startInfo.Arguments = $"/k {commandStr}";
        startInfo.Verb = "runas";
        p.StartInfo = startInfo;
        return p;
    }
}