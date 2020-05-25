
using Library;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace App
{
    public class Program
    {
        [STAThread] // required for Windows Forms
        public static void Main(string[] args)
        {
            string[] services = new string[] {
                "vmcompute"
            };
            new SysTrayIcon(services).Run();
        }
    }    
}