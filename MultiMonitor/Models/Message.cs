using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiMonitor.Models
{
    public class Message
    {
        public MessageType MessageType { get; set; }
        public object data { get; set; }
    }

    public enum MessageType
    {
        Keyboard,
        Mouse,
    }
}
