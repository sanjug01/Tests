namespace RdClient.Shared.CxWrappers
{
    using System;

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
            EmitEvent(sender, this.OperationInProgress, args);
        }

        public void EmitOperationCompleted(IRadcClient sender, RadcOperationCompletedArgs args)
        {

            EmitEvent(sender, this.OperationCompleted, args);
        }

        public void EmitWorkspaceRemoved(IRadcClient sender, RadcWorkspaceRemovedArgs args)
        {
            EmitEvent(sender, this.WorkspaceRemoved, args);
        }

        public void EmitAddResourcesStarted(IRadcClient sender, RadcAddResourcesStartedArgs args)
        {
            EmitEvent(sender, this.AddResourcesStarted, args);
        }

        public void EmitResourceAdded(IRadcClient sender, RadcResourceAddedArgs args)
        {
            EmitEvent(sender, this.ResourceAdded, args);
        }

        public void EmitAddResourcesFinished(IRadcClient sender, RadcAddResourcesFinishedArgs args)
        {
            EmitEvent(sender, this.AddResourcesFinished, args);
        }

        private void EmitEvent<T>(object sender, EventHandler<T> handler, T args) where T : EventArgs
        {
            if (handler != null)
            {
                handler(sender, args);
            }
        }
    }
}
