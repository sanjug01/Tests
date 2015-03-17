using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.CxWrappers
{
    public class RadcOperationInProgressArgs : EventArgs
    {}

    public class RadcOperationCompletedArgs : EventArgs
    {}

    public class RadcWorkspaceRemovedArgs : EventArgs
    { }

    public class RadcAddResourcesStartedArgs : EventArgs
    { }

    public class RadcResourceAddedArgs : EventArgs
    { }

    public class RadcAddResourcesFinishedArgs : EventArgs
    { }

    public interface IRadcEvents
    {
        event EventHandler<RadcOperationInProgressArgs> OperationInProgress;
        event EventHandler<RadcOperationCompletedArgs> OperationCompleted;
        event EventHandler<RadcWorkspaceRemovedArgs> WorkspaceRemoved;
        event EventHandler<RadcAddResourcesStartedArgs> AddResourcesStarted;
        event EventHandler<RadcResourceAddedArgs> ResourceAdded;
        event EventHandler<RadcAddResourcesFinishedArgs> AddResourcesFinished;
    }

    public interface IRadcEventSource
    {
        void EmitOperationInProgress(IRadcClient sender, RadcOperationInProgressArgs args);
        void EmitOperationCompleted(IRadcClient sender, RadcOperationCompletedArgs args);
        void EmitWorkspaceRemoved(IRadcClient sender, RadcWorkspaceRemovedArgs args);
        void EmitAddResourcesStarted(IRadcClient sender, RadcAddResourcesStartedArgs args);
        void EmitResourceAdded(IRadcClient sender, RadcResourceAddedArgs args);
        void EmitAddResourcesFinished(IRadcClient sender, RadcAddResourcesFinishedArgs args);
    }
}
