using RdClient.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.CxWrappers
{
    public class RdpUtils
    {
        public static void ApplyRdpProperties(
            IRdpProperties properties, 
            Desktop desktop)
        {
            properties.SetStringProperty("Full Address", desktop.hostName);
        }
    }
}
