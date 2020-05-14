using System;
using System.ServiceProcess;

class Program
{
    public static String GetWindowsServiceStatus(String seriveName)
    {
        string status = "";
        try
        {
            ServiceController sc = new ServiceController(seriveName);
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


    static void Main(string[] args)
    {
        string commandStr = string.Join(" ", args);
        if (args.Length == 1)
        {
            string name = args[0];
            string status = GetWindowsServiceStatus(name);
            Console.WriteLine($"{name} is {status}");
        }        
    }
}

