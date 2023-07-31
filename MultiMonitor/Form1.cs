using MultiMonitor.Models;
using System.Runtime.InteropServices;

namespace MultiMonitor
{
    public partial class Form1 : Form
    {
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_LBUTTONUP = 0x0202;
        private const int WM_RBUTTONDOWN = 0x0204;
        private const int WM_RBUTTONUP = 0x0205;
        private const int WM_MOUSEWHEEL = 0x020A;
        private const int WM_MOUSEMOVE = 0x0200;

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern short GetAsyncKeyState(int nVirtKey);



        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        public bool SendToServer { get; set; }
        public Hook KeyboardHook { get; set; }
        public Hook MouseHook { get; set; }

        public Client Client { get; set; }
        public Form1()
        {
            InitializeComponent();

            this.FormClosing += Form1_FormClosing;
            this.KeyboardHook = new Hook(13, OnKeyboardHookFired);
            this.MouseHook = new Hook(14, OnMouseHookFired);
            this.Client = new Client();
            this.SendToServer = false;

            this.KeyboardHook.SetHook();
            this.MouseHook.SetHook();
        }

        private void OnKeyboardHookFired(HookResponse response)
        {
            if (response.nCode >= 0 && response.wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(response.lParam);
                char key = (char)vkCode;

                if (vkCode == 0x2E)
                {
                    this.SendToServer = !this.SendToServer;
                    this.KeyboardHook.SendToServer = this.SendToServer;
                    this.MouseHook.SendToServer = this.SendToServer;

                    this.label1.Text = this.SendToServer.ToString();
                }

                if (!this.SendToServer) return;

                Client.SendMessage(new Keyboard { KeyStroke = key.ToString() });
            }
        }

        private void OnMouseHookFired(HookResponse response)
        {
            if (!this.SendToServer) return;

            if (response.nCode >= 0)
            {
                int msg = response.wParam.ToInt32();
                switch (msg)
                {
                    case WM_LBUTTONDOWN:
                        Client.SendMessage(new Mouse { Button = "WM_LBUTTONDOWN" });
                        break;
                    case WM_LBUTTONUP:
                        Client.SendMessage(new Mouse { Button = "WM_LBUTTONUP" });
                        break;
                    case WM_RBUTTONDOWN:
                        Client.SendMessage(new Mouse { Button = "WM_RBUTTONDOWN" });
                        break;
                    case WM_RBUTTONUP:
                        Client.SendMessage(new Mouse { Button = "WM_RBUTTONUP" });
                        break;
                    case WM_MOUSEMOVE:
                        MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(response.lParam, typeof(MSLLHOOKSTRUCT));
                        int x = hookStruct.pt.X;
                        int y = hookStruct.pt.Y;
                        Client.SendMessage(new MouseMove { X = x, Y = y });
                        break;
                        //case WM_MOUSEWHEEL:
                        //    int rawDelta = (int)((response.lParam.ToInt32() >> 16) & 0xFFFF);
                        //    int delta = rawDelta / 120;

                        //    if (delta > 0)
                        //    {
                        //        Client.SendMessage(new MouseMove { Direction = Direction.Up });
                        //    }
                        //    else if (delta < 0)
                        //    {
                        //        Client.SendMessage(new MouseMove { Direction = Direction.Down });
                        //    }
                        //    break;
                }
            }
        }

        private void Form1_FormClosing(object? sender, FormClosingEventArgs e)
        {
            this.KeyboardHook.Unhook();
            this.MouseHook.Unhook();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // AllocConsole();
        }
    }
}