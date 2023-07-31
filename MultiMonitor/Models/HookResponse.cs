using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiMonitor.Models
{
    public class HookResponse
    {
        public int nCode { get; set; }
        public IntPtr wParam { get; set; }
        public IntPtr lParam { get; set; }   
    }
}
