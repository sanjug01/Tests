using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Windows.Test.LifeTimeManager
{
    interface IWindowContent
    {
        public IWindowContent Content { get; set; }
    }
}
