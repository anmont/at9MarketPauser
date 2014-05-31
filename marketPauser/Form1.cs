using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections;
using System.IO;

namespace marketPauser
{
    public partial class Form1 : Form
    {
        #region imports

        [DllImport("user32.dll")]
        private static extern int RegisterHotKey(IntPtr hwnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll")]
        private static extern int UnregisterHotKey(IntPtr hwnd, int id);

        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, uint lpBaseAddress, byte[] lpBuffer, int nSize, out IntPtr lpNumberOfBytesWritten);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, uint lpBaseAddress, byte[] lpBuffer, int nSize, out int lpNumberOfBytesWritten);

        //testing
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SendMessage(int hWnd, int msg, int wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        static extern short MapVirtualKey(int wCode, int wMapType);

        // ByteSigScan convention.

        [DllImport("ByteSigScan.dll", EntryPoint = "InitializeSigScan", CallingConvention = CallingConvention.Cdecl)]
        public static extern void InitializeSigScan(uint iPID, [MarshalAs(UnmanagedType.LPStr)] string szModule);

        [DllImport("ByteSigScan.dll", EntryPoint = "SigScan", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 SigScan([MarshalAs(UnmanagedType.LPStr)] string Pattern, int Offset);

        [DllImport("ByteSigScan.dll", EntryPoint = "FinalizeSigScan", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FinalizeSigScan();

        // Kernel dll convention.

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall)]
        static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, uint dwProcessId);


        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CloseHandle(IntPtr hObject);
        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        static extern bool ReadProcessMemory(IntPtr hProcess, uint lpBaseAddress, ref uint lpBuffer, int dwSize, int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        static extern bool ReadProcessMemory(IntPtr hProcess, uint lpBaseAddress, ref float lpBuffer, int dwSize, int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        static extern bool ReadProcessMemory(IntPtr hProcess, uint lpBaseAddress, ref string lpBuffer, int dwSize, int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        static extern bool ReadProcessMemory(IntPtr hProcess, uint lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, int lpNumberOfBytesRead);
        #endregion

        //globals
        IntPtr hProc;
        double previous = 0;
        bool buying = true;
        bool enabled = false;
        KeyboardHook hook = new KeyboardHook();
        System.Media.SoundPlayer Aresume = new System.Media.SoundPlayer("resumed.wav");
        System.Media.SoundPlayer Asuspend = new System.Media.SoundPlayer("suspend.wav");
        System.Media.SoundPlayer Asell = new System.Media.SoundPlayer("sell_mode.wav");
        System.Media.SoundPlayer Abuy = new System.Media.SoundPlayer("buy_mode.wav");
        System.Media.SoundPlayer Atrade = new System.Media.SoundPlayer("trade_now.wav");

        enum ProcessAccessFlags : uint
        {
            PROCESS_ALL_ACCESS = 0x001F0FFF,
            PROCESS_CREATE_THREAD = 0x00000002,
            PROCESS_DUP_HANDLE = 0x00000040,
            PROCESS_QUERY_INFORMATION = 0x00000400,
            PROCESS_SET_INFORMATION = 0x00000200,
            PROCESS_TERMINATE = 0x00000001,
            PROCESS_VM_OPERATION = 0x00000008,
            PROCESS_VM_READ = 0x00000010,
            PROCESS_VM_WRITE = 0x00000020,
            SYNCHRONIZE = 0x00100000
        }

        public Form1()
        {
            InitializeComponent();
            
        }

        private void comboBox1_SelectedIndexChanged(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void Form1_Closing(object sender, FormClosingEventArgs e)
        {
            hook.Dispose();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            Process HandleP;
            Process[] ProcessList = Process.GetProcesses();
            String ProcessName;
            int count = 0;
            uint currentPID = 0;

            comboBox1.Items.Clear();

            for (int i = 0; i < ProcessList.Length; i++)
            {
                ProcessName = ProcessList[i].ProcessName;

                if (ProcessName == "ATrain9g")
                {
                        comboBox1.Items.Add("Atrain" + " - " + ProcessList[i].Id);
                        comboBox1.SelectedIndex = 0;
                        currentPID = (uint)Convert.ToUInt32(ProcessList[i].Id);
                        HandleP = System.Diagnostics.Process.GetProcessById(ProcessList[i].Id);
                        //gamedll = (uint)Module.BaseAddress.ToInt32();
                }
            }
            hProc = OpenProcess(ProcessAccessFlags.PROCESS_ALL_ACCESS, false, currentPID);

            
            hook.KeyPressed += new EventHandler<KeyPressedEventArgs>(hook_KeyPressed);
            hook.RegisterHotKey(2, Keys.PageUp);
            hook.RegisterHotKey(2, Keys.PageDown);
            


        }

        void hook_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            if (e.Key.ToString() == "Next")
            {
                //turn on/off
                button2_Click(this, e);
            }

            if (e.Key.ToString() == "PageUp")
            {
                //switch mode
                button1_Click(this, e);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            double marketVal = 4294967200;
            uint maketVal = 0;
            double difference = 0;
            double actual = 0;
            
            ReadProcessMemory(hProc, 0x7AAFE0, ref maketVal, 4, 0);

            if (maketVal > marketVal)
            {
                difference = (maketVal - marketVal) -96;
                actual = difference;
            }
            else
            {
                actual = maketVal;
            }

            if (!buying)
            {
                if (actual == Convert.ToInt32(txtBuy.Text) && previous == Convert.ToInt32(txtBuy.Text) - 1)
                {
                    previous = 0;
                    pauseMe();
                }
            }
            else
            {
                if (actual == Convert.ToInt32(txtSell.Text) && previous == Convert.ToInt32(txtSell.Text) + 1)
                {
                    previous = 0;
                    pauseMe();
                }
            }
            previous = actual;
            lblVal.Text = actual.ToString();
            
        }
        public void pauseMe()
        {
            var key = BitConverter.GetBytes(0);
            int bytesWritten;
            WriteProcessMemory(hProc, 0x7AA850, key, 4,out bytesWritten);
            WriteProcessMemory(hProc, 0x3411864, key, 4, out bytesWritten);
            WriteProcessMemory(hProc, 0x3411860, key, 4, out bytesWritten);
            Atrade.Play();
            verifyUse();
        }

        private void verifyUse()
        {
            timer2.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (buying)
            {
                buying = false;
                lblbuysell.Text = "sell stocks";
                Asell.Play();
            }
            else
            {
                buying = true;
                lblbuysell.Text = "buy stocks";
                Abuy.Play();
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            if (enabled)
            {
                enabled = false;
                timer1.Enabled = false;
                button2.Text = "Start";
                Asuspend.Play();
            }
            else
            {
                enabled = true;
                timer1.Enabled = true;
                button2.Text = "Stop";
                Aresume.Play();
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            uint val = 5;
            ReadProcessMemory(hProc, 0x3411864, ref val, 4, 0);

            if (val == 0)
            {
                //obviously we have stayed paused for some time... so switch modes
                button1_Click(this, e);
            }
            timer2.Enabled = false;
        }

    }

    public sealed class KeyboardHook : IDisposable
    {
        // Registers a hot key with Windows.
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        // Unregisters the hot key with Windows.
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        /// <summary>
        /// Represents the window that is used internally to get the messages.
        /// </summary>
        private class Window : NativeWindow, IDisposable
        {
            private static int WM_HOTKEY = 0x0312;

            public Window()
            {
                // create the handle for the window.
                this.CreateHandle(new CreateParams());
            }

            /// <summary>
            /// Overridden to get the notifications.
            /// </summary>
            /// <param name="m"></param>
            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);

                // check if we got a hot key pressed.
                if (m.Msg == WM_HOTKEY)
                {
                    // get the keys.
                    Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);
                    ModifierKeys modifier = (ModifierKeys)((int)m.LParam & 0xFFFF);

                    // invoke the event to notify the parent.
                    if (KeyPressed != null)
                        KeyPressed(this, new KeyPressedEventArgs(modifier, key));
                }
            }

            public event EventHandler<KeyPressedEventArgs> KeyPressed;

            #region IDisposable Members

            public void Dispose()
            {
                this.DestroyHandle();
            }

            #endregion
        }

        private Window _window = new Window();
        private int _currentId;

        public KeyboardHook()
        {
            // register the event of the inner native window.
            _window.KeyPressed += delegate(object sender, KeyPressedEventArgs args)
            {
                if (KeyPressed != null)
                    KeyPressed(this, args);
            };
        }

        /// <summary>
        /// Registers a hot key in the system.
        /// </summary>
        /// <param name="modifier">The modifiers that are associated with the hot key.</param>
        /// <param name="key">The key itself that is associated with the hot key.</param>
        public void RegisterHotKey(int modifier, Keys key)
        {
            // increment the counter.
            _currentId = _currentId + 1;

            // register the hot key.
            if (!RegisterHotKey(_window.Handle, _currentId, (uint)modifier, (uint)key))
                throw new InvalidOperationException("Couldn’t register the hot key.");
        }

        /// <summary>
        /// A hot key has been pressed.
        /// </summary>
        public event EventHandler<KeyPressedEventArgs> KeyPressed;

        #region IDisposable Members

        public void Dispose()
        {
            // unregister all the registered hot keys.
            for (int i = _currentId; i > 0; i--)
            {
                UnregisterHotKey(_window.Handle, i);
            }

            // dispose the inner native window.
            _window.Dispose();
        }

        #endregion
    }

    /// <summary>
    /// Event Args for the event that is fired after the hot key has been pressed.
    /// </summary>
    public class KeyPressedEventArgs : EventArgs
    {
        private ModifierKeys _modifier;
        private Keys _key;

        internal KeyPressedEventArgs(ModifierKeys modifier, Keys key)
        {
            _modifier = modifier;
            _key = key;
        }

        public ModifierKeys Modifier
        {
            get { return _modifier; }
        }

        public Keys Key
        {
            get { return _key; }
        }
    }

    /// <summary>
    /// The enumeration of possible modifiers.
    /// </summary>
    [Flags]
    public enum ModifierKeys : uint
    {
        Alt = 1,
        Control = 2,
        Shift = 4,
        Win = 8
    }

}
