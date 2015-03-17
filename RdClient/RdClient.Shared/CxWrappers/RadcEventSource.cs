using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.CxWrappers
{
    class RadcEventSource : IRadcEvents, IRadcEventSource
    {
        public event EventHandler<RadcOperationInProgressArgs> OperationInProgress;

        public event EventHandler<RadcOperationCompletedArgs> OperationCompleted;

        public event EventHandler<RadcWorkspaceRemovedArgs> WorkspaceRemoved;

        public event EventHandler<RadcAddResourcesStartedArgs> AddResourcesStarted;

        public event EventHandler<RadcResourceAddedArgs> ResourceAdded;

        public event EventHandler<RadcAddResourcesFinishedArgs> AddResourcesFinished;

        public void EmitOperationInProgress(IRadcClient sender, RadcOperationInProgressArgs args)
        {
            var handler = this.OperationInProgress;
            if (handler != null)
            {
                handler(sender, args);
            }
        }

        public void EmitOperationCompleted(IRadcClient sender, RadcOperationCompletedArgs args)
        {
            var handler = this.OperationCompleted;
            if (handler != null)
            {
                handler(sender, args);
            }
        }

        public void EmitWorkspaceRemoved(IRadcClient sender, RadcWorkspaceRemovedArgs args)
        {
            var handler = this.WorkspaceRemoved;
            if (handler != null)
            {
                handler(sender, args);
            }
        }

        public void EmitAddResourcesStarted(IRadcClient sender, RadcAddResourcesStartedArgs args)
        {
            var handler = this.AddResourcesStarted;
            if (handler != null)
            {
                handler(sender, args);
            }
        }

        public void EmitResourceAdded(IRadcClient sender, RadcResourceAddedArgs args)
        {
            var handler = this.ResourceAdded;
            if (handler != null)
            {
                handler(sender, args);
            }
        }

        public void EmitAddResourcesFinished(IRadcClient sender, RadcAddResourcesFinishedArgs args)
        {
            var handler = this.AddResourcesFinished;
            if (handler != null)
            {
                handler(sender, args);
            }
        }
    }
}
