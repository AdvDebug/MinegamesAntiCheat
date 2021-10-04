using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

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

        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern IntPtr NtSetInformationThread(IntPtr ThreadHandle, uint ThreadInfoClass, IntPtr ThreadInfo, uint ThreadInfoLength);

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenThread(uint dwDesiredAccess, bool bInheritHandle, uint dwThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetCurrentThread();

        private static void AntiUnHooker_Normal()
        {
            while (true)
            {
                IntPtr KernelModule = GetModuleHandle("kernelbase.dll");
                IntPtr LoadLibraryWAddress = GetProcAddress(KernelModule, "LoadLibraryW");
                IntPtr LoadLibraryAAddress = GetProcAddress(KernelModule, "LoadLibraryA");
                byte[] LoadLibraryWCode = new byte[1];
                byte[] LoadLibraryACode = new byte[1];
                Marshal.Copy(LoadLibraryWAddress, LoadLibraryWCode, 0, 1);
                Marshal.Copy(LoadLibraryAAddress, LoadLibraryACode, 0, 1);
                if (LoadLibraryWCode[0] == 0xCC == false || LoadLibraryACode[0] == 0xCC == false)
                {
                    Environment.Exit(0);
                }
            }
        }

        private static void AntiUnHooker_Aggerssive()
        {
            while (true)
            {
                Task.Delay(500).Wait();
                IntPtr KernelModule = GetModuleHandle("kernelbase.dll");
                IntPtr NtdllModule = GetModuleHandle("ntdll.dll");
                IntPtr LoadLibraryWAddress = GetProcAddress(KernelModule, "LoadLibraryW");
                IntPtr LoadLibraryAAddress = GetProcAddress(KernelModule, "LoadLibraryA");
                IntPtr LoadLibraryExAAddress = GetProcAddress(KernelModule, "LoadLibraryExA");
                IntPtr LoadLibraryExWAddress = GetProcAddress(KernelModule, "LoadLibraryExW");
                IntPtr LdrLoadDllAddress = GetProcAddress(NtdllModule, "LdrLoadDll");
                byte[] LoadLibraryWCode = new byte[1];
                byte[] LoadLibraryACode = new byte[1];
                byte[] LoadLibraryExACode = new byte[1];
                byte[] LoadLibraryExWCode = new byte[1];
                byte[] LdrLoadDllCode = new byte[1];
                Marshal.Copy(LoadLibraryWAddress, LoadLibraryWCode, 0, 1);
                Marshal.Copy(LoadLibraryAAddress, LoadLibraryACode, 0, 1);
                Marshal.Copy(LoadLibraryExAAddress, LoadLibraryExACode, 0, 1);
                Marshal.Copy(LoadLibraryExWAddress, LoadLibraryExWCode, 0, 1);
                Marshal.Copy(LdrLoadDllAddress, LdrLoadDllCode, 0, 1);
                if(LoadLibraryACode[0] == 0xCC == false || LoadLibraryWCode[0] == 0xCC == false || LoadLibraryExACode[0] == 0xCC == false || LoadLibraryExWCode[0] == 0xCC == false || LdrLoadDllCode[0] == 0xCC == false)
                {
                    Environment.Exit(0);
                }
            }
        }

        private static void AntiUnHook(bool IsAggressiveMode)
        {
            if (IsAggressiveMode == false)
            {
                Thread AntiUnHook_Normal_Thread = new Thread(new ThreadStart(AntiUnHooker_Normal));
                AntiUnHook_Normal_Thread.Start();
            }
            else
            {
                Thread AntiUnHook_Aggerssive_Thread = new Thread(new ThreadStart(AntiUnHooker_Aggerssive));
                AntiUnHook_Aggerssive_Thread.Start();
            }
        }

        public static void LockDownLibraryLoading_Normal(bool EnableAntiUnHooker)
        {
            IntPtr KernelModule = GetModuleHandle("kernelbase.dll");
            IntPtr LoadLibraryWAddress = GetProcAddress(KernelModule, "LoadLibraryW");
            IntPtr LoadLibraryAAddress = GetProcAddress(KernelModule, "LoadLibraryA");
            byte[] INT3InvaildCode = { 0xCC };
            WriteProcessMemory(Process.GetCurrentProcess().Handle, LoadLibraryWAddress, INT3InvaildCode, 6, 0);
            WriteProcessMemory(Process.GetCurrentProcess().Handle, LoadLibraryAAddress, INT3InvaildCode, 6, 0);
            if (EnableAntiUnHooker)
            {
                AntiUnHook(false);
            }
        }

        public static void LockDownLibraryLoading_Aggressive(bool EnableAntiUnHooker)
        {
            IntPtr KernelModule = GetModuleHandle("kernelbase.dll");
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
            if(EnableAntiUnHooker)
            {
                AntiUnHook(true);
            }
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