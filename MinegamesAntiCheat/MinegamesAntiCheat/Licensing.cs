using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Security.Cryptography;
using System.IO;

namespace MinegamesAntiCheat
{
    public class Licensing
    {
        private static string HashingHardwareID(string ToHash)
        {
            byte[] KeyToHashWith = Encoding.ASCII.GetBytes("bAI!J6XwWO&A");
            HMACSHA256 SHA256Hashing = new HMACSHA256();
            SHA256Hashing.Key = KeyToHashWith;
            var TheHash = SHA256Hashing.ComputeHash(UTF8Encoding.UTF8.GetBytes(ToHash));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < TheHash.Length; i++)
            {
                builder.Append(TheHash[i].ToString("x2"));
            }
            string FinalHash = builder.ToString();
            return FinalHash;
        }

        public static string GetHardwareID()
        {
            ManagementObjectSearcher CPU = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
            ManagementObjectCollection GetCPU = CPU.Get();
            string CPUID = null;
            foreach (ManagementObject CPUId in GetCPU)
            {
                CPUID = CPUId["ProcessorType"].ToString() + CPUId["ProcessorId"].ToString();
            }
            ManagementObjectSearcher BIOS = new ManagementObjectSearcher("SELECT * FROM Win32_BIOS");
            ManagementObjectCollection GetBIOS = BIOS.Get();
            string GPUID = null;
            foreach (ManagementObject BIOSId in GetBIOS)
            {
                GPUID = BIOSId["Manufacturer"].ToString() + BIOSId["Version"].ToString();
            }
            return HashingHardwareID(CPUID + GPUID);
        }

        public static string GetHDDOrUSB_HardwareID(string DeviceID)
        {
            ManagementObjectSearcher Drive = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
            ManagementObjectCollection GetDrive = Drive.Get();
            foreach(ManagementObject GetHWID in GetDrive)
            {
                if(GetHWID["DeviceID"].ToString() == DeviceID)
                {
                    return HashingHardwareID(GetHWID["Model"].ToString() + GetHWID["SerialNumber"].ToString() + GetHWID["Signature"].ToString() + GetHWID["Manufacturer"].ToString());
                }
            }
            return null;
        }
    }
}