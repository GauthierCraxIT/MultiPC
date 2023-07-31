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
        public TcpClient _tcpClient { get; set; }
        public List<string> _MessageQueue { get; set; }
        public Client()
        {
            _tcpClient = new TcpClient();
            _tcpClient.Connect("localhost", 7000);

            _MessageQueue = new List<string>();
            Run();
        }

        private void Run()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    Thread.Sleep(1);
                    while (_MessageQueue.Count > 0)
                    {
                        var stream = _tcpClient.GetStream();

                        var buffer = Encoding.ASCII.GetBytes(_MessageQueue[0]);
                        await stream.WriteAsync(buffer, 0, buffer.Length);

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
