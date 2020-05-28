
using Library;
using System;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;

namespace App
{
    public class Program
    {
        [STAThread] // required for Windows Forms
        public static void Main(string[] args)
        {
            SysTrayOption[] options = new SysTrayOption[] {
                new SysTrayOption("vmcompute"),
                new SysTrayOption("HvHost"),
                new SysTrayOption("Show Boot Mode", "bcdedit")
            };
            new SysTrayIcon(options).Run();
        }
    }    
}