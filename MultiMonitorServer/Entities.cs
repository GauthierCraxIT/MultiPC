using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiMonitorServer
{
    public class MouseEvent
    {
        public string Button { get; set; }
    }

    public class MouseMoveEvent
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
    public class KeyboardEvent
    {
        public string KeyStroke { get; set; }
    }

}
