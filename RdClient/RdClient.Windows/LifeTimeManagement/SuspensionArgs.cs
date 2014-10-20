using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace RdClient.LifeTimeManagement
{
    public class MySuspendingOperation : ISuspendingOperationWrapper
    {
        public DateTimeOffset Deadline
        { get; set; }

        public ISuspendingDeferral Deferral
        { get; set; }
    }

    public class SuspensionArgs : ISuspensionArgs
    {
        public ISuspendingOperationWrapper SuspendingOperation
        { get; set; }
    }
}
