using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;

namespace MinegamesAntiCheat
{
    public class AntiDebugging
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool IsDebuggerPresent();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CheckRemoteDebuggerPresent(IntPtr Handle, ref bool CheckBool);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr Handle);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lib);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr Module, string Function);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteProcessMemory(IntPtr ProcHandle, IntPtr BaseAddress, byte[] Buffer, uint size, int NumOfBytes);

        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern IntPtr NtSetInformationThread(IntPtr ThreadHandle, uint ThreadInfoClass, IntPtr ThreadInfo, uint ThreadInfoLength);

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenThread(uint dwDesiredAccess, bool bInheritHandle, uint dwThreadId);

        public static bool CloseHandleAntiDebug()
        {
            try
            {
                CloseHandle((IntPtr)0xD99121L);
                return false;
            }
            catch (Exception ex)
            {
                if (ex.Message == "External component has thrown an exception.")
                {
                    return true;
                }
            }
            return false;
        }

        public static bool RemoteDebuggerCheckAntiDebug()
        {
            bool RemoteDebugCheck = false;
            CheckRemoteDebuggerPresent(Process.GetCurrentProcess().Handle, ref RemoteDebugCheck);
            if (RemoteDebugCheck || Debugger.IsAttached || Debugger.IsLogging() || IsDebuggerPresent())
                return true;
            return false;
        }

        private static void AntiDebuggingThread()
        {
            while (true)
            {
                Task.Delay(1500).Wait();
                if (RemoteDebuggerCheckAntiDebug() || CloseHandleAntiDebug())
                {
                    Environment.Exit(0);
                }
            }
        }

        public static void AntiDebuggerAttach()
        {
            IntPtr NtdllModule = GetModuleHandle("ntdll.dll");
            IntPtr DbgUiRemoteBreakinAddress = GetProcAddress(NtdllModule, "DbgUiRemoteBreakin");
            IntPtr DbgUiConnectToDbgAddress = GetProcAddress(NtdllModule, "DbgUiConnectToDbg");
            byte[] Int3InvaildCode = { 0xCC };
            WriteProcessMemory(Process.GetCurrentProcess().Handle, DbgUiRemoteBreakinAddress, Int3InvaildCode, 6, 0);
            WriteProcessMemory(Process.GetCurrentProcess().Handle, DbgUiConnectToDbgAddress, Int3InvaildCode, 6, 0);
        }

        public static void ActiveAntiDebuggingProtection()
        {
            Thread ProtectionThread = new Thread(new ThreadStart(AntiDebuggingThread));
            ProtectionThread.Start();
        }

        public static void HideThreadsFromDebugger()
        {
            var ProcThreads = Process.GetCurrentProcess().Threads;
            foreach (ProcessThread ThreadsInProc in ProcThreads)
            {
                IntPtr ThreadsHandle = OpenThread(0x0020, false, (uint)ThreadsInProc.Id);
                NtSetInformationThread(ThreadsHandle, 0x11, IntPtr.Zero, 0);
            }
        }
    }
}