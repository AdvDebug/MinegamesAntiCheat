using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MinegamesAntiCheat;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;

namespace MinegamesAntiCheatAPP
{
    public partial class Main : Form
    {

        public Main()
        {
            InitializeComponent();
        }

        private bool IsDllVaild()
        {
            if (File.Exists(Environment.CurrentDirectory + @"\MinegamesAntiCheat.dll"))
            {
                FileStream CheckIfDllVaild = new FileStream(Environment.CurrentDirectory + @"\MinegamesAntiCheat.dll", FileMode.Open, FileAccess.Read);
                SHA256CryptoServiceProvider HashFile = new SHA256CryptoServiceProvider();
                byte[] FileHashSHA256 = HashFile.ComputeHash(CheckIfDllVaild);
                string TheFinalHashSHA256 = BitConverter.ToString(FileHashSHA256).Replace("-", string.Empty).ToLower();
                if (TheFinalHashSHA256 == "7b2be263068452a462c0a1dbca54650eb6241f120d2532c22a729524b476d44b")
                {
                    CheckIfDllVaild.Close();
                    HashFile.Clear();
                    return true;
                }
            }
            return false;
        }

        private void PreventReplacing(bool IsWillUse)
        {
            FileStream OpenFileToPreventReplacing = new FileStream(Environment.CurrentDirectory + @"\MinegamesAntiCheat.dll", FileMode.Open, FileAccess.Read);
            if (IsWillUse == true)
            {
                OpenFileToPreventReplacing.Close();
            }
        }

        private void AntiDllInjectionThread()
        {
            Task.Delay(7000).Wait();
            AntiCheat.LockDownLibraryLoading();
            AntiCheat.PreventManualMappingDllInjection();
            AntiDebugging.ActiveAntiDebuggingProtection();
        }

        private void LeavingWindowThread()
        {
            AntiCheat.PreventLeavingOrNotFocusingOnWindowHandle(this.Handle);
        }

        private void Main_Load(object sender, EventArgs e)
        {
            if (IsDllVaild() == false)
            {
                MessageBox.Show("Unfixable Error while loading a library, please contact the software developer and try again.", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                Environment.Exit(0);
                Task.Delay(1000).Wait();
            }
            AntiDebugging.AntiDebuggerAttach();
            if (AntiDebugging.CloseHandleAntiDebug() || AntiDebugging.RemoteDebuggerCheckAntiDebug())
            {
                MessageBox.Show("Yes, there's a debugger.", "Debugger Detected", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("nope, no debugger detected.", "Nope", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            }

            if (AntiVirtualization.IsSandboxiePresent() || AntiVirtualization.IsWinePresent() || AntiVirtualization.IsEmulationPresent() || AntiVirtualization.IsVMPresent())
            {
                MessageBox.Show("Yes, you are in a virutal environment.", "Virtual Environment Detected", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Nope, no virtual environment detected.", "Not a Virtual Environment", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            }
            PreventReplacing(true);
            Thread DllInjectionPreventionThread = new Thread(new ThreadStart(AntiDllInjectionThread));
            DllInjectionPreventionThread.Start();
            if(DllInjectionPreventionThread.ManagedThreadId.ToString() == "3")
            {
                MessageBox.Show("Possible Debugger Detection.", "Debugger", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            }
            AntiDebugging.HideThreadsFromDebugger();
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
