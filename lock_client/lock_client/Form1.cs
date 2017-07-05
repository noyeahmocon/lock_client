using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lock_client
{
    public partial class Form1 : Form
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_SYSKEYUP = 0x0105;
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static int _hookID = 0;
        private delegate int LowLevelKeyboardProc(int nCode, int wParam, ref KBDLLHOOKSTRUCT IParam);
        private static int HookCallback(int nCode, int wParam, ref KBDLLHOOKSTRUCT IParam)
        {
            bool bReturn = false;
            switch (wParam)
            {
                case WM_KEYDOWN:
                case WM_KEYUP:
                case WM_SYSKEYDOWN:
                case WM_SYSKEYUP:

                    bReturn = ((IParam.vkCode == 0x09) && (IParam.flags == 0x20)) ||    //Alt + Tab
                        ((IParam.vkCode == 0x1B) && (IParam.flags == 0x20)) ||  //Alt + Esc
                        ((IParam.vkCode == 0x1B) && (IParam.flags == 0x00)) ||  //Ctrl + Esc
                        ((IParam.vkCode == 0x5B) && (IParam.flags == 0x01)) ||  //Left Windows Key
                        ((IParam.vkCode == 0x5C) && (IParam.flags == 0x01)) ||  //Right Windows Key
                        ((IParam.vkCode == 0x73) && (IParam.flags == 0x20));    //Alt + F4

                    break;
            }
            if (bReturn == true)
                return 1;
            else
                return CallNextHookEx(0, nCode, wParam, ref IParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int SetWindowsHookEx(int idHook, LowLevelKeyboardProc Ipfn, IntPtr hMod, uint dwThredId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(int hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int CallNextHookEx(int hhk, int nCode, int wParam, ref KBDLLHOOKSTRUCT IParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string IpModulName);

        public struct KBDLLHOOKSTRUCT
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }
        //

        private static int SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);

            }
        }

        //Ctrl + Alt + Delete 막기
        public static void KillCtrlAltDelete()
        {
            RegistryKey regkey;
            string keyValueInt = "1";
            string subKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System";

            try
            {
                regkey = Registry.CurrentUser.CreateSubKey(subKey);
                regkey.SetValue("DisableTaskMgr", keyValueInt);
                regkey.Close();
            }
            catch (Exception ex)
            {
                //  MessageBox.Show(ex.ToString());
            }
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Module.DisableTaskManager();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Module.EnableTaskManager();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            KillCtrlAltDelete();
            _hookID = SetHook(_proc);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //Focus();
            //Activate();
            label1.Text = DateTime.Now.ToString("hh:mm");
            label2.Text = DateTime.Now.ToString("yyyy-MM-dd, ddd");
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
