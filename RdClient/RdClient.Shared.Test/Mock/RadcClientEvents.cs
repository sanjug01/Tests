namespace RdClient.Shared.Test.Mock
{
    using RdClient.Shared.CxWrappers;
    using RdMock;
    using System;

    public class RadcClientEvents : MockBase, IRadcEvents
    {
        #pragma warning disable 67 // warning CS0067: the event <...> is never used - they shouldn't be as this is a mock object
        public event EventHandler<RadcAddResourcesFinishedArgs> AddResourcesFinished;
        public event EventHandler<RadcAddResourcesStartedArgs> AddResourcesStarted;
        public event EventHandler<RadcOperationCompletedArgs> OperationCompleted;
        public event EventHandler<RadcOperationInProgressArgs> OperationInProgress;
        public event EventHandler<RadcResourceAddedArgs> ResourceAdded;
        public event EventHandler<RadcWorkspaceRemovedArgs> WorkspaceRemoved;
    }
}
