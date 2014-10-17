using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace RdClient.LifeTimeManagement
{
    public interface IMySuspendingOperation
    {
        DateTimeOffset Deadline { get; set; }

        ISuspendingDeferral Deferral { get; set; }
    }
    public interface ISuspensionArgs
    {
        IMySuspendingOperation SuspendingOperation { get; set; }
    }
}
