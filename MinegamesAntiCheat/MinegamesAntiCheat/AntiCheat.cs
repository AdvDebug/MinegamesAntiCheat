using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using System.Threading;

namespace MinegamesAntiCheat
{
    public class AntiCheat
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lib);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr Module, string Function);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteProcessMemory(IntPtr ProcHandle, IntPtr BaseAddress, byte[] Buffer, uint size, int NumOfBytes);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(IntPtr ProcHandle, IntPtr BaseAddress, byte[] Buffer, uint size, int NumOfBytes);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetForegroundWindow(IntPtr WindowHandle);

        public static void LockDownLibraryLoading()
        {
            IntPtr KernelModule = GetModuleHandle("kernel32.dll");
            IntPtr NtdllModule = GetModuleHandle("ntdll.dll");
            IntPtr LoadLibraryWAddress = GetProcAddress(KernelModule, "LoadLibraryW");
            IntPtr LoadLibraryAAddress = GetProcAddress(KernelModule, "LoadLibraryA");
            IntPtr LoadLibraryExAAddress = GetProcAddress(KernelModule, "LoadLibraryExA");
            IntPtr LoadLibraryExWAddress = GetProcAddress(KernelModule, "LoadLibraryExW");
            IntPtr LdrLoadDllAddress = GetProcAddress(NtdllModule, "LdrLoadDll");
            byte[] Int3InvaildCode = { 0xCC };
            WriteProcessMemory(Process.GetCurrentProcess().Handle, LoadLibraryWAddress, Int3InvaildCode, 6, 0);
            WriteProcessMemory(Process.GetCurrentProcess().Handle, LoadLibraryAAddress, Int3InvaildCode, 6, 0);
            WriteProcessMemory(Process.GetCurrentProcess().Handle, LoadLibraryExAAddress, Int3InvaildCode, 6, 0);
            WriteProcessMemory(Process.GetCurrentProcess().Handle, LoadLibraryExWAddress, Int3InvaildCode, 6, 0);
            WriteProcessMemory(Process.GetCurrentProcess().Handle, LdrLoadDllAddress, Int3InvaildCode, 6, 0);
        }

        public static void PreventManualMappingDllInjection()
        {
            IntPtr KernelModule = GetModuleHandle("kernel32.dll");
            IntPtr WriteProcessMemoryAddress = GetProcAddress(KernelModule, "WriteProcessMemory");
            byte[] Code = { 204, 235, 32, 2, 0, 204, 161, 24, 0, 0, 0, 51, 201, 131, 236 };
            WriteProcessMemory(Process.GetCurrentProcess().Handle, WriteProcessMemoryAddress, Code, 6, 0);
        }

        public static void PreventLeavingOrNotFocusingOnWindowHandle(IntPtr WindowHandle)
        {
            while(true)
            {
                Task.Delay(1500).Wait();
                if(WindowHandle == GetForegroundWindow(WindowHandle) == false)
                {
                    Environment.Exit(0);
                }
            }
        }
    }
}