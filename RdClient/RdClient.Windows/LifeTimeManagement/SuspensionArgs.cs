using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace RdClient.LifeTimeManagement
{
    public class MySuspendingOperation : IMySuspendingOperation
    {
        public DateTimeOffset Deadline
        { get; set; }

        public ISuspendingDeferral Deferral
        { get; set; }
    }

    public class SuspensionArgs : ISuspensionArgs
    {
        public IMySuspendingOperation SuspendingOperation
        { get; set; }
    }
}
