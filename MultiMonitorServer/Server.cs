using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace MultiMonitorServer
{
    public class Server
    {
        [DllImport("user32.dll")]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const uint MOUSEEVENTF_LEFTUP = 0x0004;
        private const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const uint MOUSEEVENTF_RIGHTUP = 0x0010;

        public UdpClient udpServer { get; set; }

        public Server()
        {
            this.udpServer = new UdpClient(7000);
        }

        public void RunServer()
        {
            //var client = this.TcpListener.AcceptTcpClient();
            //var ns = client.GetStream();
            //var buffer = new byte[1024];
            //while (true)
            //{
            //    ns.Read(buffer, 0, buffer.Length);
            //    var msg = Encoding.ASCII.GetString(buffer);

            //    string pattern = @"(\{[^{}]*\})";
            //    MatchCollection matches = Regex.Matches(msg, pattern);

            //    foreach (Match match in matches)
            //    {
            //        HandleIncommingMessage(match.Value);
            //    }
            //}

            while (true)
            {
                Thread.Sleep(1);
                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] receivedData = udpServer.Receive(ref remoteEndPoint);

                Console.WriteLine(receivedData.Length);

                string msg = Encoding.ASCII.GetString(receivedData);

                Console.WriteLine($"Received: {msg} from {remoteEndPoint}");
                string pattern = @"(\{[^{}]*\})";
                MatchCollection matches = Regex.Matches(msg, pattern);

                foreach (Match match in matches)
                {
                    Console.WriteLine(match.Value);
                    //HandleIncommingMessage(match.Value);
                }
            }
        }

        private void HandleIncommingMessage(string msg)
        {
            if (msg.Contains("Button"))
            {
                var obj = JsonSerializer.Deserialize<MouseEvent>(msg);
                HandlMouseMessage(obj);
            }
            else if (msg.Contains("KeyStroke"))
            {
                var obj = JsonSerializer.Deserialize<KeyboardEvent>(msg);
                HandleKeyboardMessage(obj);
            } else
            {
                var obj = JsonSerializer.Deserialize<MouseMoveEvent>(msg);
                HandleMouseMoveMessage(obj);
            }
        }

        private void HandleKeyboardMessage(KeyboardEvent e)
        {
            SendKeys.SendWait(e.KeyStroke);
        }

        private void HandleMouseMoveMessage(MouseMoveEvent e)
        {
            SetCursorPos(e.X, e.Y);
        }

        private void HandlMouseMessage(MouseEvent e)
        {
            switch (e.Button)
            {
                case "WM_LBUTTONDOWN":
                    {
                        mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                        break;
                    }
                case "WM_LBUTTONUP":
                    {
                        mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                        break;
                    }
                case "WM_RBUTTONDOWN":
                    {
                        mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
                        break;
                    }
                case "WM_RBUTTONUP":
                    {
                        mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
                        break;
                    }
            }
        }
    }
}
