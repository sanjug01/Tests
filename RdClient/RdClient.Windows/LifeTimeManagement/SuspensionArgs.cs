using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace RdClient.LifeTimeManagement
{
    public class SuspensionArgs : ISuspensionArgs
    {
        public class SuspendingOperationWrapper : ISuspendingOperationWrapper
        {
            private DateTimeOffset _deadLine;
            public DateTimeOffset Deadline { get { return _deadLine; } }

            private ISuspendingDeferral _deferral;
            public ISuspendingDeferral Deferral { get { return _deferral; } }

            public SuspendingOperationWrapper(DateTimeOffset deadLine, ISuspendingDeferral deferral)
            {
                _deadLine = deadLine;
                _deferral = deferral;
            }
        }

        private ISuspendingOperationWrapper _suspendingOperation;

        public ISuspendingOperationWrapper SuspendingOperation
        { get { return _suspendingOperation; } }

        public SuspensionArgs(ISuspendingOperationWrapper suspendingOperation)
        {
            _suspendingOperation = suspendingOperation;
        }
    }
}

