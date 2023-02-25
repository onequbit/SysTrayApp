using System;
using System.Management;

public class Program
{
    public static void Main()
    {
        ManagementPath managementPath = new ManagementPath();
        managementPath.Path = "root\\WMI:BcdStore.Id=\"{fa926493-6f1c-4193-a414-58f0b2456d1e}\"";
        string wmiClassName = "BcdStore";
        ManagementScope managementScope = new ManagementScope(managementPath);
        ObjectQuery objectQuery = new ObjectQuery("SELECT * FROM " + wmiClassName);
        ManagementObjectSearcher objectSearcher = new ManagementObjectSearcher(managementScope, objectQuery);
        ManagementObjectCollection objectCollection = objectSearcher.Get();
        foreach (ManagementObject managementObject in objectCollection)
        {
            PropertyDataCollection props = managementObject.Properties;
            foreach (PropertyData prop in props)
            {
                Console.WriteLine("Property name: {0}", prop.Name);
                Console.WriteLine("Property type: {0}", prop.Type);
                Console.WriteLine("Property value: {0}", prop.Value);
            }
        }
        // // var a = new ManagementObject("Win32_LogicalDisk.DeviceID='D:'");            
        // // Console.WriteLine(a.GetPropertyValue("VolumeSerialNumber").ToString());
        // ConnectionOptions connectionOptions = new ConnectionOptions();
        // connectionOptions.Impersonation = ImpersonationLevel.Impersonate;
        // connectionOptions.EnablePrivileges = true;

        // // The ManagementScope is used to access the WMI info as Administrator
        // ManagementScope managementScope = new ManagementScope(@"root\WMI", connectionOptions);

        // // {9dea862c-5cdd-4e70-acc1-f32b344d4795} is the GUID of the System BcdStore
        // // ManagementObject privateLateBoundObject = new ManagementObject(managementScope, new ManagementPath("root\\WMI:BcdObject.Id=\"{9dea862c-5cdd-4e70-acc1-f32b344d4795}\",StoreFilePath=\"\""), null);
        // ManagementObject privateLateBoundObject = new ManagementObject(managementScope, new ManagementPath("root\\WMI:BcdStore"), null);
        

        // foreach (PropertyData prop in privateLateBoundObject.Properties)
        // {
        //     Console.WriteLine("{0}: {1}", prop.Name, prop.Value);
        // }

        // ManagementBaseObject inParams = null;
        // inParams = privateLateBoundObject.GetMethodParameters("GetElement");

        // // 0x24000001 is a BCD constant: BcdBootMgrObjectList_DisplayOrder
        // inParams["Type"] = ((UInt32)0x24000001);
        // ManagementBaseObject outParams = privateLateBoundObject.InvokeMethod("GetElement", inParams, null);
        // ManagementBaseObject mboOut = ((ManagementBaseObject)(outParams.Properties["Element"].Value));

        // string[] osIdList = (string[]) mboOut.GetPropertyValue("Ids");

        // // Each osGuid is the GUID of one Boot Manager in the BcdStore
        // foreach (string osGuid in osIdList)
        // {
        //     ManagementObject currentManObj = new ManagementObject(managementScope, new ManagementPath("root\\WMI:BcdObject.Id=\"" + osGuid + "\",StoreFilePath=\"\""), null);
        //     Console.WriteLine("" + currentManObj.GetPropertyValue("Id"));
        // }
    }

}

