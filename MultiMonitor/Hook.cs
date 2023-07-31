using MultiMonitor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MultiMonitor
{
    public class Hook
    {
        private delegate IntPtr LowLevelProc(int nCode, IntPtr wParam, IntPtr lParam);

        public bool SendToServer { get; set; }
        private int _hookTypeId { get; set; }
        private Action<HookResponse> _onHookFired { get; set; }

        private LowLevelProc _keyboardProc;
        private IntPtr _hookID = IntPtr.Zero;

        private const int WM_MOUSEMOVE = 0x0200;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        public Hook(int hookTypeId, Action<HookResponse> onHookFired)
        {
            _hookTypeId = hookTypeId;
            _onHookFired = onHookFired;
            SendToServer = false;
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            _onHookFired(new HookResponse
            {
                lParam = lParam,
                nCode = nCode,
                wParam = wParam,
            });

            int msg = wParam.ToInt32();
            if (msg == WM_MOUSEMOVE)
            {
                return CallNextHookEx(_hookID, nCode, wParam, lParam);
            }

            return SendToServer ? (IntPtr)1 : CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        public void SetHook()
        {
            _keyboardProc = HookCallback;
            _hookID = SetWindowsHookEx(_hookTypeId, _keyboardProc, IntPtr.Zero, 0);
        }

        public void Unhook()
        {
            UnhookWindowsHookEx(_hookID);
        }
    }
}
