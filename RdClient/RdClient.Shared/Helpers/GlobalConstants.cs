using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.Helpers
{
    public static class GlobalConstants
    {
        public static readonly ulong MaxDoubleTapUS = 300000; // microseconds      
        public static readonly double DesiredDeceleration = 0.002;
    }
}
