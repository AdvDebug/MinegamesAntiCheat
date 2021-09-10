using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Microsoft.Win32;
using System.Management;
using System.Threading;
using System.ServiceProcess;

namespace MinegamesAntiCheat
{
    public class AntiVirtualization
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetModuleHandleW(string lib);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr ModuleHandle, string Function);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern void Sleep(int MilliSec);

        public static bool IsSandboxiePresent()
        {
            if (GetModuleHandleW("SbieDll.dll").ToInt32() != 0)
                return true;
            return false;
        }

        public static bool IsEmulationPresent()
        {
            long Tick = Environment.TickCount;
            Thread.Sleep(500);
            long Tick2 = Environment.TickCount;
            if (((Tick2 - Tick) < 500L))
            {
                return true;
            }
            return false;
        }

        public static bool IsWinePresent()
        {
            IntPtr ModuleHandle = GetModuleHandleW("kernel32.dll");
            if (GetProcAddress(ModuleHandle, "wine_get_unix_file_name").ToInt32() != 0)
                return true;
            return false;
        }

        public static bool IsVMPresent()
        {
            using (var searcher = new ManagementObjectSearcher("Select * from Win32_ComputerSystem"))
            {
                using (var items = searcher.Get())
                {
                    foreach (var item in items)
                    {
                        string manufacturer = item["Manufacturer"].ToString().ToLower();
                        if ((manufacturer == "microsoft corporation" && item["Model"].ToString().ToUpperInvariant().Contains("VIRTUAL"))
                            || manufacturer.Contains("vmware")
                            || item["Model"].ToString() == "VirtualBox")
                        {
                            return true;
                        }
                    }
                }
            }
            ServiceController[] services = ServiceController.GetServices();
            foreach (ServiceController service in services)
            {
                string[] Services = { "vmbus", "VMBusHID", "hyperkbd" };
                foreach (string CheckServices in Services)
                {
                    if (service.ServiceName.Contains(CheckServices))
                        return true;
                }
            }
            return false;
        }
    }
}