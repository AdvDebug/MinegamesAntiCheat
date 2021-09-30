using System;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Drawing;

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

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool VirtualProtect(IntPtr Address, uint Size, uint NewProtect, uint OldProtect);

        public static void LockDownLibraryLoading_Normal()
        {
            IntPtr KernelModule = GetModuleHandle("kernel32.dll");
            IntPtr LoadLibraryWAddress = GetProcAddress(KernelModule, "LoadLibraryW");
            IntPtr LoadLibraryAAddress = GetProcAddress(KernelModule, "LoadLibraryA");
            byte[] INT3InvaildCode = { 0xCC };
            WriteProcessMemory(Process.GetCurrentProcess().Handle, LoadLibraryWAddress, INT3InvaildCode, 6, 0);
            WriteProcessMemory(Process.GetCurrentProcess().Handle, LoadLibraryAAddress, INT3InvaildCode, 6, 0);
        }

        public static void LockDownLibraryLoading_Aggerssive()
        {
            IntPtr KernelModule = GetModuleHandle("kernel32.dll");
            IntPtr NtdllModule = GetModuleHandle("ntdll.dll");
            IntPtr LoadLibraryWAddress = GetProcAddress(KernelModule, "LoadLibraryW");
            IntPtr LoadLibraryAAddress = GetProcAddress(KernelModule, "LoadLibraryA");
            IntPtr LoadLibraryExAAddress = GetProcAddress(KernelModule, "LoadLibraryExA");
            IntPtr LoadLibraryExWAddress = GetProcAddress(KernelModule, "LoadLibraryExW");
            IntPtr LdrLoadDllAddress = GetProcAddress(NtdllModule, "LdrLoadDll");
            byte[] INT3InvaildCode = { 0xCC };
            WriteProcessMemory(Process.GetCurrentProcess().Handle, LoadLibraryWAddress, INT3InvaildCode, 6, 0);
            WriteProcessMemory(Process.GetCurrentProcess().Handle, LoadLibraryAAddress, INT3InvaildCode, 6, 0);
            WriteProcessMemory(Process.GetCurrentProcess().Handle, LoadLibraryExAAddress, INT3InvaildCode, 6, 0);
            WriteProcessMemory(Process.GetCurrentProcess().Handle, LoadLibraryExWAddress, INT3InvaildCode, 6, 0);
            WriteProcessMemory(Process.GetCurrentProcess().Handle, LdrLoadDllAddress, INT3InvaildCode, 6, 0);
        }

        public static void PreventLeavingOrNotFocusingOnWindowHandle(IntPtr WindowHandle)
        {
            while (true)
            {
                Task.Delay(1500).Wait();
                if (WindowHandle == GetForegroundWindow(WindowHandle) == false)
                {
                    Environment.Exit(0);
                }
            }
        }

        public enum Protections
        {
            PAGE_NOACCESS = 0x01,
            PAGE_READONLY = 0x02,
            PAGE_READWRITE = 0x04,
            PAGE_WRITECOPY = 0x08,
            PAGE_GUARD = 0x100,
            PAGE_NOCACHE = 0x200,
            PAGE_WRITECOMBINE = 0x400,
            PAGE_TARGETS_INVALID = 0x40000000,
        };

        public static void ChangeMemoryPageAccess(IntPtr Address, uint Size, uint NewProtection)
        {
            uint OldProtect = 0;
            VirtualProtect(Address, Size, NewProtection, OldProtect);
        }

        public static void InjectCodeToFunction(byte[] AssemblyCode, string LibraryOfFunction, string Function)
        {
            IntPtr LibraryModule = GetModuleHandle(LibraryOfFunction);
            IntPtr FunctionAddress = GetProcAddress(LibraryModule, Function);
            WriteProcessMemory(Process.GetCurrentProcess().Handle, FunctionAddress, AssemblyCode, 6, 0);
        }
    }
}