using Library;
using System;
using System.Collections.Generic;
using System.ServiceProcess;

class Program
{

    static void Main()
    {
        Console.WriteLine("foo!");
        var services = ServiceController.GetServices();
        string[] names = services.ToNames();
        names.ToMessageBox();
        bool test = names.Contains("vmcompute");
        Console.WriteLine($"names.Contains('vmcompute') == {test}");
    }
}

