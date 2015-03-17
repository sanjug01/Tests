namespace RdClient.Shared.CxWrappers
{
    using RdClient.Shared.CxWrappers.Errors;
    using System;

    public class RadcOperationInProgressArgs : EventArgs
    {
        public RadcFeedOperation OperationType { get; private set; }

        public RadcOperationInProgressArgs(RadcFeedOperation operationType)
        {
            this.OperationType = operationType;
        }
    }

    public class RadcOperationCompletedArgs : EventArgs
    {
        public RadcError.RadcStatus Result { get; private set; }

        public RadcOperationCompletedArgs(RadcError.RadcStatus result)
        {
            this.Result = result;
        }
    }

    public class RadcWorkspaceRemovedArgs : EventArgs
    {
        public string FeedUrl { get; private set; }

        public RadcWorkspaceRemovedArgs(string feedUrl)
        {
            this.FeedUrl = feedUrl;
        }
    }

    public class RadcAddResourcesStartedArgs : EventArgs
    { 
        public string FeedUrl { get; private set; }

        public RadcAddResourcesStartedArgs(string feedUrl)
        {
            this.FeedUrl = feedUrl;
        }        
    }

    public class RadcResourceAddedArgs : EventArgs
    {
        public string WorkspaceId { get; private set; }
        public string WorkspaceFriendlyName { get; private set; }
        public string FeedUrl { get; private set; }
        public RemoteResourceType ResourceType { get; private set; }
        public string ResourceId { get; private set; }
        public string FriendlyName { get; private set; }
        public string RdpFile { get; private set; }
        public byte[] IconBytes { get; private set; }
        public uint IconWidth { get; private set; }

        public RadcResourceAddedArgs(string workspaceId, string workspaceFriendlyName, string feedUrl, string resourceId, string friendlyName, string rdpFile, RemoteResourceType resourceType, byte[] iconBytes, uint iconWidth)
        {
            this.WorkspaceId = workspaceId;
            this.WorkspaceFriendlyName = workspaceFriendlyName;
            this.FeedUrl = feedUrl;
            this.ResourceType = resourceType;
            this.ResourceId = resourceId;
            this.FriendlyName = friendlyName;
            this.RdpFile = rdpFile;
            this.IconBytes = iconBytes;
            this.IconWidth = iconWidth;
        }

    }

    public class RadcAddResourcesFinishedArgs : EventArgs
    { 
        public string FeedUrl { get; private set; }

        public RadcAddResourcesFinishedArgs(string feedUrl)
        {
            this.FeedUrl = feedUrl;
        }    
    }

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
