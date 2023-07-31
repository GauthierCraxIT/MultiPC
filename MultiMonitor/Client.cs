using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MultiMonitor
{
    public class Client
    {
        public UdpClient udpClient { get; set; }
        public List<string> _MessageQueue { get; set; }
        public Client()
        {
            udpClient = new UdpClient();
            _MessageQueue = new List<string>();
            Run();
        }

        private void Run()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(1);
                    while (_MessageQueue.Count > 0)
                    {
                        var buffer = Encoding.ASCII.GetBytes(_MessageQueue[0]);
                        udpClient.Send(buffer, buffer.Length, "localhost", 7000);
                        _MessageQueue.RemoveAt(0);
                    }
                }
            });
        }

        public void SendMessage<T>(T message)
        {
            _MessageQueue.Add(JsonSerializer.Serialize(message));
        }
    }
}
